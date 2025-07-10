using AppWeb.Infrastructure.Persistence.Repositories;
using AppWeb.Application.Graphql.Filters;
using AppWeb.Infrastructure.Persistence;
using AppWeb.Application.Graphql.Cqrs;
using AppWeb.Application.Helpers;
using AppWeb.Domain.Interfaces;
using AppWeb.Domain.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using AppWeb.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using AppWeb.Application.Security;
using System.Text;

namespace AppWeb.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //Connection string configuration
        services.AddPooledDbContextFactory<AppDBContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        //Security helpers
        services.AddSingleton<Microsoft.AspNetCore.Identity.IPasswordHasher<string>, Microsoft.AspNetCore.Identity.PasswordHasher<string>>();
        services.AddSingleton<IJwtGenerator, JwtGenerator>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); //Generic repository registration
        services.AddAllRepositories(typeof(IUserRepository).Assembly, typeof(UserRepository).Assembly);

        // Configure authentication with both Cookie and JWT support
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        { // Cookie authentication for server-side pages
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/AuthDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        })
        .AddJwtBearer(options =>
        { // JWT authentication for API endpoints
            var key = configuration["Jwt:Secret"] ?? configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key)) throw new InvalidOperationException("JWT secret key is missing in configuration");
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"] ?? "AppWeb.Server",
                ValidAudience = configuration["Jwt:Audience"] ?? "AppWeb.Client",
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(key))
            };
            
            // We don't need to save the token as we're using the authorization header
            options.SaveToken = false;
            
            // Don't automatically challenge, as we're using a mix of cookie and JWT auth
            options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            {
                OnMessageReceived = context =>
                { //Allow JWT tokens in the Authorization header or as a cookie named "jwt"
                    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last() ??
                                context.Request.Cookies["jwt"]; //Get token from cookie                    
                    if (!string.IsNullOrEmpty(token)) context.Token = token;
                    return Task.CompletedTask;
                }
            };
        });

        services.AddGraphQLServer() //GraphQL Server
                .RegisterDbContextFactory<AppDBContext>()
                .AddQueryType(d => d.Name("Query"))
                .AddMutationType(d => d.Name("Mutation"))
                .AddAllModelTypes(typeof(User).Assembly, "AppWeb.Domain.Models")
                .AddAllExtensions(typeof(UserQuery).Assembly, "AppWeb.Application.Graphql.Cqrs")
                .AddAllExtensions(typeof(UserCommand).Assembly, "AppWeb.Application.Graphql.Cqrs")
                .AddProjections().AddFiltering().AddSorting()
                .AddErrorFilter<ValidationExceptionFilter>()
                .InitializeOnStartup();
        return services;
    }
}
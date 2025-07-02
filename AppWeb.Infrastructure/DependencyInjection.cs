using AppWeb.Infrastructure.Persistence.Repositories;
using AppWeb.Application.Graphql.Filters;
using AppWeb.Infrastructure.Persistence;
using AppWeb.Application.Graphql.Cqrs;
using AppWeb.Application.Helpers;
using AppWeb.Domain.Interfaces;
using AppWeb.Domain.Models;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AppWeb.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using AppWeb.Application.Security;

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

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        { //Authentication (Cookie)
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/AuthDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
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
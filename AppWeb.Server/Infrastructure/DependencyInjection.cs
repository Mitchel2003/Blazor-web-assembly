using AppWeb.Server.Infrastructure.Persistence.Repositories;
using AppWeb.Server.Application.Interfaces.Persistence;
using AppWeb.Server.Application.Graphql.Mutations;
using AppWeb.Server.Application.Graphql.Queries;
using AppWeb.Server.Application.Graphql.Filters;
using AppWeb.Server.Infrastructure.Persistence;
using AppWeb.Server.Application.Helpers;
using AppWeb.Server.Models;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace AppWeb.Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //Connection string configuration
        services.AddPooledDbContextFactory<AppDBContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("conection")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); //Generic repository registration
        services.AddAllRepositories(typeof(IUserRepository).Assembly, typeof(UserRepository).Assembly);

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        { //Authentication (Cookie)
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/AuthDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        });

        services.AddGraphQLServer() //GraphQL Server (HotChocolate)
                .RegisterDbContextFactory<AppDBContext>()
                .AddQueryType(d => d.Name("Query"))
                .AddMutationType(d => d.Name("Mutation"))
                .AddAllModelTypes(typeof(User).Assembly, "AppWeb.Server.Models")
                .AddAllExtensions(typeof(UserQuery).Assembly, "AppWeb.Server.Application.Graphql.Queries")
                .AddAllExtensions(typeof(UserMutation).Assembly, "AppWeb.Server.Application.Graphql.Mutations")
                .AddProjections().AddFiltering().AddSorting()
                .AddErrorFilter<ValidationExceptionFilter>()
                .InitializeOnStartup();
        return services;
    }
}
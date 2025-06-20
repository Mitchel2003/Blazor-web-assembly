using AppWeb.Infrastructure.Persistence.Repositories;
using AppWeb.Application.Graphql.Mutations;
using AppWeb.Application.Graphql.Queries;
using AppWeb.Infrastructure.Persistence;
using AppWeb.Application.Helpers;
using AppWeb.Models;

using AppWeb.Application.Contracts.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using AppWeb.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppWeb.Infrastructure;

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
                .AddAllModelTypes(typeof(User).Assembly, "AppWeb.Models")
                .AddAllExtensions(typeof(UserQuery).Assembly, "AppWeb.Application.Graphql.Queries")
                .AddAllExtensions(typeof(UserMutation).Assembly, "AppWeb.Application.Graphql.Mutations")
                .AddProjections().AddFiltering().AddSorting()
                .InitializeOnStartup();
        return services;
    }
}
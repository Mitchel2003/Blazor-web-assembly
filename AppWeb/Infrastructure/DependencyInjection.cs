using Microsoft.AspNetCore.Authentication.Cookies;
using AppWeb.Application.Graphql.Mutations;
using AppWeb.Application.Graphql.Queries;
using Microsoft.EntityFrameworkCore;
using AppWeb.Application.Shared;
using AppWeb.Models;
using AppWeb.Domain;
using AppWeb.Core;

namespace AppWeb.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //Connection string configuration
        services.AddPooledDbContextFactory<AppDBContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("conection")));

        //GraphQL helpers (generic Mutation<T>/Query<T>)
        services.AddScoped(typeof(Query<>));
        services.AddScoped(typeof(Mutation<>));

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
                .AddProjections().AddFiltering().AddSorting();
        return services;
    }
}
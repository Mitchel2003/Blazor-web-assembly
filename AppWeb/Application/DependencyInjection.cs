using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;

namespace AppWeb.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    { //Registers all MediatR handlers in the current assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
}

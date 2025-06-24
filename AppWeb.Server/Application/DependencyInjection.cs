using AppWeb.Server.Application.Mapping;
using AppWeb.Server.Application.Core;
using System.Reflection;
using FluentValidation;
using MediatR;

namespace AppWeb.Server.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    { //Registers all MediatR handlers, validators (FluentValidation), and mapper (Mapster)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); //Validator behavior
        services.AddMapping(); //Registers Mapster configuration
        return services;
    }
}
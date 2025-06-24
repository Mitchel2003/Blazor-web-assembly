using System.Reflection;
using MapsterMapper;
using Mapster;

namespace AppWeb.Server.Application.Mapping;

/// <summary>
/// Centralized Mapster configuration. Scans the Application assembly for all <see cref="IRegister"/> implementations
/// and registers <see cref="ServiceMapper"/> for DI. This keeps mapping rules in one place and avoids duplicated files.
/// </summary>
public static class MapsterConfig
{
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly()); //Dynamically scan for IRegister

        config.Compile(); //Optionally pre-compile for performance in Production
        services.AddSingleton(config); //Register the configuration for DI
        services.AddScoped<IMapper, ServiceMapper>(); //mapper for DI
        return services;
    }
}
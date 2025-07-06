using AppWeb.Client.Helpers;
using MudBlazor.Services;

namespace AppWeb.Client;

/// <summary>Configures dependency injection for client-side services.</summary>
public static class DependencyInjection
{
    /// <summary>Adds all client-side services, HTTP clients, and ViewModels.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiBase">Optional base address for API clients.</param>
    /// <returns>The service collection with client services registered.</returns>
    public static IServiceCollection AddClient(this IServiceCollection services, Uri? apiBase = null)
    {
        services.AddMudServices(); //Add UI framework services (singleton por defecto)
        
        //Add all client services using our centralized extension methods
        // Esto registra servicios con los ciclos de vida apropiados:
        // - Singleton: para servicios sin estado compartidos entre todos los usuarios
        // - Scoped: para servicios con estado por sesión de usuario
        // - Transient: para servicios ligeros que se crean bajo demanda
        services.AddClientServices(apiBase);
        
        // Register ViewModels from AppWeb.ViewModels
        // Los ViewModels se registran como Transient para que cada componente
        // reciba su propia instancia y evitar problemas de estado compartido
        ViewModels.DependencyInjection.AddViewModels(services);
        return services;
    }
}
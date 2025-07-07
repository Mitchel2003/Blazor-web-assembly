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
        services.AddMudServices(); //Add UI framework services (singleton by default)
        
        //Add all client services using our centralized extension methods
        // This registers services with appropriate lifecycles:
        // - Singleton: for stateless services shared across all users
        // - Scoped: for stateful services per user session
        // - Transient: for lightweight services created on demand
        services.AddClientServices(apiBase);
        
        // Register ViewModels from AppWeb.ViewModels
        // ViewModels are registered as Scoped to maintain state within a user session
        // while avoiding shared state issues between different users
        AppWeb.ViewModels.DependencyInjection.AddViewModels(services);
        
        // No need to register client-side ViewModels as they're now in AppWeb.ViewModels project
        
        return services;
    }
}
using AppWeb.Client.Helpers;
using MudBlazor.Services;

namespace AppWeb.Client;

/// <summary>
/// Registers all client-side services, HTTP clients, and feature ViewModels automatically.
/// Mirrors the AddApplication / AddInfrastructure helpers on the server side.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Adds client-side services.</summary>
    /// <param name="services">DI collection.</param>
    /// <param name="apiBase">Optional base address for <see cref="HttpClient"/>.</param>
    public static IServiceCollection AddClient(this IServiceCollection services, Uri? apiBase = null)
    {
        services.AddMudServices();
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        services.AddScoped<Errors.ErrorNotifier>();
        
        services.AddAuthServices();
        services.AddHttpClients(apiBase);
        services.AddViewModels();
        return services;
    }
}
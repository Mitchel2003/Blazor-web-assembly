using AppWeb.ViewModels.Core.Services;
using AppWeb.Client.Services.Blazor;
using AppWeb.Client.Services;
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

        // Registrar implementaciones específicas de Blazor
        services.AddScoped<INavigationService, BlazorNavigationService>();
        services.AddScoped<IMessageService, BlazorMessageService>();
        services.AddScoped<IUsersService, BlazorUsersService>();

        // Registrar ViewModels compartidos desde la biblioteca AppWeb.ViewModels
        // Importante: usar el namespace completo para evitar ambigüedades
        ViewModels.DependencyInjection.AddViewModels(services);
        
        return services;
    }
}
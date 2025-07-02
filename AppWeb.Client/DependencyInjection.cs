using Microsoft.AspNetCore.Components.Authorization;
using AppWeb.Client.Services;
using MudBlazor.Services;
using System.Reflection;

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
        if (apiBase is not null)
        {
            services.AddScoped(sp => new HttpClient { BaseAddress = apiBase });
            //Add the JWT handler to all HTTP requests
            services.AddScoped<Auth.JwtHandler>();
            services.AddHttpClient("", client => client.BaseAddress = apiBase).AddHttpMessageHandler<Auth.JwtHandler>();
        }
        services.AddMudServices();
        services.AddAuthorizationCore();
        services.AddScoped<Auth.JwtAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider, Auth.JwtAuthStateProvider>();
        services.AddCascadingAuthenticationState();

        services.AddHttpClient<IUsersApiClient, UsersApiClient>(client => { if (apiBase != null) client.BaseAddress = apiBase; }).AddHttpMessageHandler<Auth.JwtHandler>();
        services.AddHttpClient<IAuthApiClient, AuthApiClient>(client => { if (apiBase != null) client.BaseAddress = apiBase; }).AddHttpMessageHandler<Auth.JwtHandler>();
        
        RegisterViewModels(services); //Register every *VM class in *.ViewModels.
        services.AddScoped<Errors.ErrorNotifier>(); //Shared client-side helpers.
        return services;
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var vms = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("VM", StringComparison.Ordinal) && t.Namespace?.EndsWith("ViewModels", StringComparison.Ordinal) == true);
        foreach (var vm in vms) { services.AddScoped(vm); }
    }
}
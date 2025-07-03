using Microsoft.AspNetCore.Components.Authorization;
using AppWeb.Client.Services;
using AppWeb.Client.Auth;
using System.Reflection;

namespace AppWeb.Client.Helpers;

/// <summary>Extension methods for IServiceCollection to register client-side services.</summary>
public static class ServiceCollectionExtensions
{
    #region Authentication ------------------------------------------------------------
    /// <summary>Registers authentication-related services.</summary>
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<JwtAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
        services.AddScoped<JwtHandler>();
        return services;
    }
    #endregion ---------------------------------------------------------------------

    #region Api client ------------------------------------------------------------
    /// <summary>Registers HTTP clients with optional base address.</summary>
    public static IServiceCollection AddHttpClients(this IServiceCollection services, Uri? apiBase = null)
    {
        if (apiBase is not null)
        {
            services.AddScoped(sp => new HttpClient { BaseAddress = apiBase });
            services.AddHttpClient("", client => client.BaseAddress = apiBase).AddHttpMessageHandler<JwtHandler>();
        }
        
        // Register specific API clients
        services.AddHttpClient<IUsersApiClient, UsersApiClient>(client => { if (apiBase != null) client.BaseAddress = apiBase; }).AddHttpMessageHandler<JwtHandler>();
        services.AddHttpClient<IAuthApiClient, AuthApiClient>(client => { if (apiBase != null) client.BaseAddress = apiBase; }).AddHttpMessageHandler<JwtHandler>();
        return services;
    }
    #endregion ---------------------------------------------------------------------

    #region Current ------------------------------------------------------------
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    { //Registers all ViewModels in the current assembly
        var assembly = Assembly.GetExecutingAssembly();
        RegisterViewModelsByNamespace(services, assembly, "Pages");
        RegisterViewModelsByNamespace(services, assembly, "Components");
        RegisterComponentViewModels(services, assembly); //Register component ViewModels
        return services;
    }
    
    private static void RegisterViewModelsByNamespace(IServiceCollection services, Assembly assembly, string namespaceSuffix)
    { //Registers ViewModels in a specific namespace ending with the given suffix
        var viewModelTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("VM", StringComparison.Ordinal) && t.Namespace?.EndsWith(namespaceSuffix, StringComparison.Ordinal) == true);
        foreach (var vmType in viewModelTypes) services.AddScoped(vmType);
    }
    
    private static void RegisterComponentViewModels(IServiceCollection services, Assembly assembly)
    { //Registers component ViewModels
        var componentVmTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("VM", StringComparison.Ordinal) && t.Namespace?.Contains("Components", StringComparison.Ordinal) == true);
        foreach (var vmType in componentVmTypes) services.AddScoped(vmType);
    }
    #endregion ---------------------------------------------------------------------
}
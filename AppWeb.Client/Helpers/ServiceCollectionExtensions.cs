using Microsoft.AspNetCore.Components.Authorization;
using AppWeb.Client.Services;
using AppWeb.Client.Auth;
using System.Reflection;

namespace AppWeb.Client.Helpers;

/// <summary>Extension methods for IServiceCollection to register client-side services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers authentication-related services.</summary>
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<JwtAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
        services.AddScoped<JwtHandler>();
        return services;
    }
    
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
    
    /// <summary>Registers all ViewModels in the assembly.</summary>
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();        
        RegisterViewModelsByNamespace(services, assembly, "Pages"); //Register ViewModels in the ViewModels namespace
        RegisterViewModelsByNamespace(services, assembly, "Components"); //Register ViewModels in the ViewModels namespace
        RegisterComponentViewModels(services, assembly); //Register component ViewModels
        return services;
    }
    
    /// <summary>Registers ViewModels in namespaces ending with the specified suffix.</summary>
    private static void RegisterViewModelsByNamespace(IServiceCollection services, Assembly assembly, string namespaceSuffix)
    {
        var viewModelTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("VM", StringComparison.Ordinal) && t.Namespace?.EndsWith(namespaceSuffix, StringComparison.Ordinal) == true);
        foreach (var vmType in viewModelTypes) services.AddScoped(vmType);
    }
    
    /// <summary>Registers component ViewModels.</summary>
    private static void RegisterComponentViewModels(IServiceCollection services, Assembly assembly)
    {
        var componentVmTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("VM", StringComparison.Ordinal) && t.Namespace?.Contains("Components", StringComparison.Ordinal) == true);
        foreach (var vmType in componentVmTypes) services.AddScoped(vmType);
    }
}
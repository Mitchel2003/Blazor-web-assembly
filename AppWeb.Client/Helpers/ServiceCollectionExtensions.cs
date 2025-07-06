using Microsoft.AspNetCore.Components.Authorization;
using AppWeb.ViewModels.Core.Services;
using AppWeb.ViewModels.Core.Factory;
using AppWeb.Client.Services.Blazor;
using AppWeb.Client.Services;
using AppWeb.Client.Errors;
using AppWeb.Client.Http;
using AppWeb.Client.Auth;
using AppWeb.ViewModels;

namespace AppWeb.Client.Helpers;

/// <summary>Extension methods for IServiceCollection to register client-side services.</summary>
public static class ServiceCollectionExtensions
{
    #region Collection services ------------------------------------------------------------
    /// <summary>Registers all client services in a single call.</summary>
    public static IServiceCollection AddClientServices(this IServiceCollection services, Uri? apiBase = null)
    { return services.AddBlazorServices().AddAuthServices().AddHttpClients(apiBase).AddErrorHandling().AddPlatformServices(); }
    #endregion ---------------------------------------------------------------------

    #region Blazor services ------------------------------------------------------------
    /// <summary>Registers Blazor-specific services.</summary>
    public static IServiceCollection AddBlazorServices(this IServiceCollection services)
    {
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        return services;
    }
    #endregion ---------------------------------------------------------------------

    #region Authentication services ------------------------------------------------------------
    /// <summary>Registers authentication-related services.</summary>
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    { //Use Scoped for authentication services to maintain user session state
        services.AddScoped<JwtHandler>();
        services.AddScoped<JwtAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<JwtAuthStateProvider>());
        return services;
    }
    #endregion ---------------------------------------------------------------------

    #region Http clients ------------------------------------------------------------
    /// <summary>Registers HTTP clients with optional base address.</summary>
    public static IServiceCollection AddHttpClients(this IServiceCollection services, Uri? apiBase = null)
    { //Use HttpClientFactory to manage HTTP connections efficiently
        services.AddHttpClient();
        if (apiBase != null)
        { //Create a base HttpClient with the configured API address
            services.AddScoped(sp => new HttpClient { BaseAddress = apiBase });
            //Configure HttpClient with JWT authentication, using HttpClientFactory
            services.AddHttpClient("", client => client.BaseAddress = apiBase).AddHttpMessageHandler<JwtHandler>();
        }
        
        // Registrar clientes API específicos como servicios Scoped
        // Usar HttpClientFactory para gestionar eficientemente el ciclo de vida de los HttpClient
        services.AddHttpClient<IUsersApiClient, UsersApiClient>(client => { if (apiBase != null) client.BaseAddress = apiBase; }).AddHttpMessageHandler<JwtHandler>();
        services.AddHttpClient<IAuthApiClient, AuthApiClient>(client => { if (apiBase != null) client.BaseAddress = apiBase; }).AddHttpMessageHandler<JwtHandler>();        
        return services;
    }
    #endregion ---------------------------------------------------------------------

    #region Error handling ------------------------------------------------------------
    /// <summary>Registers error handling services.</summary>
    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
    { //Register error handling services as Scoped to maintain session context
        services.AddScoped<IApiErrorParser, ApiErrorParser>();
        services.AddScoped<ErrorNotifier>();
        return services;
    }
    #endregion ---------------------------------------------------------------------

    #region Platform services ------------------------------------------------------------
    /// <summary>Registers platform services for ViewModels.</summary>
    public static IServiceCollection AddPlatformServices(this IServiceCollection services)
    { //Register platform services as Scoped to maintain session state
        services.AddScoped<IAuthService, BlazorAuthService>();
        services.AddScoped<IMessageService, BlazorMessageService>();
        services.AddScoped<INavigationService, BlazorNavigationService>();
        services.AddScoped<IUsersService, BlazorUsersService>();
        
        //Register ModelFactory as Singleton to share between all sessions; since it is stateless and thread-safe
        services.AddSingleton<IModelFactory>(sp => DefaultModelFactory.Instance);
        return services;
    }
    #endregion ---------------------------------------------------------------------
}
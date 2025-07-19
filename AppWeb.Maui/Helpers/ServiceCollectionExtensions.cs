using Microsoft.AspNetCore.Components.Authorization;
using AppWeb.SharedClient.Services.Adapters;
using AppWeb.SharedClient.Services.Graphql;
using Microsoft.Extensions.Configuration;
using AppWeb.Shared.Services.Contracts;
using AppWeb.ViewModels.Core.Factory;
using AppWeb.Maui.Views.Users;
using AppWeb.Maui.Views.Auth;
using AppWeb.Maui.Services;
using System.Reflection;
using AppWeb.Maui.Views;
using AppWeb.Maui.Auth;

namespace AppWeb.Maui.Helpers;

/// <summary>Extension methods for IServiceCollection to register MAUI-specific services.</summary>
public static class ServiceCollectionExtensions
{
    #region Collection services ------------------------------------------------------------
    /// <summary>Registers all MAUI services in a single call.</summary>
    public static MauiAppBuilder AddMauiServices(this MauiAppBuilder builder, AppSettings appSettings = null)
    { //First configure to have the configuration available
        builder = builder.AddConfiguration();
        //If no AppSettings is provided, create a new one with the current configuration
        if (appSettings == null) { appSettings = new AppSettings(); builder.Configuration.Bind(appSettings); }

        return builder
            .AddAuthServices()
            .AddHttpClients(appSettings)
            .AddPlatformServices()
            .AddAppViews();
    }
    #endregion ---------------------------------------------------------------------

    #region Configuration ------------------------------------------------------------
    /// <summary>Registers configuration-related services.</summary>
    public static MauiAppBuilder AddConfiguration(this MauiAppBuilder builder)
    {
        var configBuilder = builder.Configuration;
        var assembly = Assembly.GetExecutingAssembly();

        try
        { //Try to load the file as an embedded resource first (appsettings.json)
            var resourceNames = assembly.GetManifestResourceNames();
            var resourceName = resourceNames.FirstOrDefault(r => r.EndsWith("appsettings.json", StringComparison.OrdinalIgnoreCase));
            if (resourceName != null)
            { //We found the embedded resource
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null) { configBuilder.AddJsonStream(stream); }
            }
            else
            { //If we don't find the embedded resource, try to load from file
                configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            }
        }
        catch (Exception ex) { configBuilder.AddInMemoryCollection(new Dictionary<string, string> { { "ApiBaseUrl", "https://localhost:7240" } }); }

        //Register AppSettings as singleton
        var appSettings = new AppSettings();
        configBuilder.Bind(appSettings);
        builder.Services.AddSingleton(appSettings);
        return builder;
    }
    #endregion ---------------------------------------------------------------------

    #region Authentication services ------------------------------------------------------------
    /// <summary>Registers authentication-related services.</summary>
    public static MauiAppBuilder AddAuthServices(this MauiAppBuilder builder)
    { //Register authentication services
        builder.Services.AddScoped<JwtAuthStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<JwtAuthStateProvider>());
        builder.Services.AddScoped<JwtHandler>();
        //Add authorization core services
        builder.Services.AddAuthorizationCore();
        return builder;
    }
    #endregion ---------------------------------------------------------------------

    #region Http clients ------------------------------------------------------------
    /// <summary>Registers HTTP clients with the API base address.</summary>
    public static MauiAppBuilder AddHttpClients(this MauiAppBuilder builder, AppSettings settings)
    { //Create HttpClient with JWT authentication handler
        builder.Services.AddSingleton(sp =>
        {
            var handler = sp.GetRequiredService<JwtHandler>();
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri(settings.ApiBaseUrl) };
            return httpClient;
        });

        //Register API clients
        builder.Services.AddSingleton<IUsersApiClient, UsersApiClient>();
        builder.Services.AddSingleton<IAuthApiClient, AuthApiClient>();
        return builder;
    }
    #endregion ---------------------------------------------------------------------

    #region Platform services ------------------------------------------------------------
    /// <summary>Registers MAUI-specific platform services.</summary>
    public static MauiAppBuilder AddPlatformServices(this MauiAppBuilder builder)
    { //Register platform-specific services
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IUsersService, UsersService>();

        //Register navigation service with route configuration
        builder.Services.AddSingleton<NavigationService>();
        builder.Services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<NavigationService>());
        builder.Services.AddSingleton<Action<NavigationService>>(ConfigureRoutes);

        builder.Services.AddSingleton<IMessageService, MessageService>();

        //Register ViewModels
        ViewModels.DependencyInjection.AddViewModels(builder.Services);

        //Register ModelFactory as Singleton
        builder.Services.AddSingleton<IModelFactory>(sp => DefaultModelFactory.Instance);

        return builder;
    }
    #endregion ---------------------------------------------------------------------

    #region App Views ------------------------------------------------------------
    /// <summary>Registers all application views/pages.</summary>
    public static MauiAppBuilder AddAppViews(this MauiAppBuilder builder)
    {
        //Register application pages are handled by the navigation service
        //The actual registration happens in the ConfigureRoutes method
        return builder;
    }
    #endregion ---------------------------------------------------------------------

    #region Helpers ------------------------------------------------------------
    /// <summary>Configures navigation routes for the MAUI application</summary>
    private static void ConfigureRoutes(NavigationService navigationService)
    { //Register page types with their corresponding routes
        navigationService.RegisterPageType(Services.NavigationConfig.Routes.Login, typeof(LoginPage));
        navigationService.RegisterPageType(Services.NavigationConfig.Routes.Home, typeof(HomePage));
        navigationService.RegisterPageType(Services.NavigationConfig.Routes.Users, typeof(TableUsersPage));
        navigationService.RegisterPageType(Services.NavigationConfig.Routes.CreateUser, typeof(UserFormPage));
        navigationService.RegisterPageType(Services.NavigationConfig.Routes.EditUser, typeof(UserFormPage));
    }
    #endregion ---------------------------------------------------------------------
}
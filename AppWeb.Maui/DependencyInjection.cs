using AppWeb.SharedClient.Services.Adapters;
using Microsoft.Extensions.Configuration;
using AppWeb.Shared.Services.Contracts;
using AppWeb.Maui.Views.Users;
using AppWeb.Maui.Views.Auth;
using AppWeb.Maui.Services;
using AppWeb.Maui.Views;
using System.Reflection;
using System.Diagnostics;

namespace AppWeb.Maui;

/// <summary>Extension methods for registering services in the MAUI dependency injection container.</summary>
public static class DependencyInjection
{
    /// <summary>Registers all application services for MAUI.</summary>
    /// <returns>The service collection for chaining</returns>
    public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly(); //Load configuration from appsettings.json
            using var stream = assembly.GetManifestResourceStream("AppWeb.Maui.appsettings.json");
            
            if (stream != null)
            {
                var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
                builder.Configuration.AddConfiguration(config);
                Debug.WriteLine("Successfully loaded appsettings.json configuration");
            }
            else { Debug.WriteLine("WARNING: Could not find embedded resource 'AppWeb.Maui.appsettings.json'"); }
        }
        catch (Exception ex) { Debug.WriteLine($"Error loading configuration: {ex.Message}"); }

        //Register app settings
        builder.Services.AddSingleton<AppSettings>();

        //Register HttpClient
        builder.Services.AddSingleton(sp =>
        { //Get the AppSettings service and return a new HttpClient
            var appSettings = sp.GetRequiredService<AppSettings>();
            return new HttpClient { BaseAddress = new Uri(appSettings.ApiBaseUrl), Timeout = TimeSpan.FromSeconds(appSettings.ApiTimeout)};
        });

        //Register Adapters (SharedClient)
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<UsersService>();

        //Register MAUI service implementations
        builder.Services.AddSingleton<MessageService>();
        builder.Services.AddSingleton<NavigationService>();

        //Register services for ViewModels (mapping MAUI implementations to interfaces)
        builder.Services.AddSingleton<IAuthService>(sp => sp.GetRequiredService<AuthService>());
        builder.Services.AddSingleton<IMessageService>(sp => sp.GetRequiredService<MessageService>());
        builder.Services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<NavigationService>());
        builder.Services.AddSingleton<IUsersService>(sp => sp.GetRequiredService<UsersService>());

        //Configure route registration
        builder.Services.AddSingleton<Action<NavigationService>>(sp => ConfigureRoutes);

        builder.Services.AddViewModels(); //Register ViewModels from AppWeb.ViewModels
        builder.Services.AddSingleton<App>(); //Register the App class with its dependencies
        return builder;
    }

    /// <summary>Configures navigation routes for the MAUI application</summary>
    private static void ConfigureRoutes(NavigationService navigationService)
    { // Register page types with their corresponding routes
        navigationService.RegisterPageType(NavigationConfig.Routes.Login, typeof(LoginPage));
        navigationService.RegisterPageType(NavigationConfig.Routes.Home, typeof(HomePage));
        navigationService.RegisterPageType(NavigationConfig.Routes.Users, typeof(TableUsersPage));
        navigationService.RegisterPageType(NavigationConfig.Routes.CreateUser, typeof(UserFormPage));
        navigationService.RegisterPageType(NavigationConfig.Routes.EditUser, typeof(UserFormPage));
    }

    /// <summary>Registers all view model services.</summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    private static IServiceCollection AddViewModels(this IServiceCollection services)
    { //Call the ViewModels project's extension method
        ViewModels.DependencyInjection.AddViewModels(services);
        return services;
    }
}
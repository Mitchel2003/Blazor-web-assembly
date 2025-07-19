using AppWeb.Maui.Helpers;

namespace AppWeb.Maui;

/// <summary>Extension methods for registering services in the MAUI dependency injection container.</summary>
public static class DependencyInjection
{
    /// <summary>Registers all application services for MAUI.</summary>
    /// <returns>The MauiAppBuilder for chaining</returns>
    public static MauiAppBuilder AddServices(this MauiAppBuilder builder)
    {
        // Use the more granular extension methods from ServiceCollectionExtensions
        return builder.AddMauiServices(null);
    }
}
namespace AppWeb.ViewModels.Core.Services;

/// <summary>
/// Abstraction of the navigation service for the ViewModels.
/// This interface provides an abstraction layer for navigation,
/// allowing the ViewModels to work both in Blazor and MAUI.
/// </summary>
/// <remarks>
/// Route Conventions:
/// - Authentication:
///   - Login: "/auth/login"
///   - Logout: "/auth/logout"
/// - User Management:
///   - User List: "/users"
///   - User Create: "/users/create"
///   - User Edit: "/users/edit/{0}" (where {0} is the user ID)
/// - Dashboard:
///   - Home: "/"
///
/// Using the NavigationConfig object is the preferred way to navigate
/// as it provides a unified approach that scales better than individual methods.
/// </remarks>

/// <summary>Configuration for navigation operations.</summary>
public class NavigationConfig
{
    /// <summary>Known application routes for easy access.</summary>
    public static class Routes
    {
        public const string Home = "/";
        public const string Login = "login";
        public const string Logout = "logout";
        public const string Users = "users";
        public const string CreateUser = "users/create";
        public const string EditUser = "users/edit";
        // Add new routes here as needed
    }

    public Dictionary<string, object> Parameters { get; }
    public bool ForceReload { get; private set; } = false;
    public bool ReplaceHistory { get; private set; }
    public string Route { get; }

    /// <summary>Creates a new navigation configuration with the specified route.</summary>
    /// <param name="route">The target route.</param>
    public NavigationConfig(string route)
    {
        Route = route;
        Parameters = new Dictionary<string, object>();
    }

    /// <summary>Adds a parameter to the navigation.</summary>
    /// <param name="key">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <returns>This instance for fluent chaining.</returns>
    public NavigationConfig WithParam(string key, object value)
    {
        Parameters[key] = value;
        return this;
    }

    /// <summary>Sets the navigation to force a reload of the target.</summary>
    /// <param name="force">Whether to force reload.</param>
    /// <returns>This instance for fluent chaining.</returns>
    public NavigationConfig WithForceLoad(bool force = true)
    {
        ForceReload = force;
        return this;
    }

    /// <summary>Sets the navigation to replace the current history entry.</summary>
    /// <param name="replace">Whether to replace history.</param>
    /// <returns>This instance for fluent chaining.</returns>
    public NavigationConfig WithReplaceHistory(bool replace = true)
    {
        ReplaceHistory = replace;
        return this;
    }
}

/// <summary>Service for handling navigation in the application.</summary>
public interface INavigationService
{
    /// <summary>Navigates to the specified route.</summary>
    /// <param name="config">Navigation configuration.</param>
    Task NavigateToAsync(NavigationConfig config);

    /// <summary>Navigates to the specified route with string.</summary>
    /// <param name="route">Target route as string.</param>
    Task NavigateToAsync(string route);

    /// <summary>Gets a query parameter from the current route.</summary>
    /// <param name="paramName">The name of the parameter.</param>
    /// <returns>The parameter value or null if not found.</returns>
    string? GetQueryParam(string paramName);

    /// <summary>Checks if the user is authenticated.</summary>
    /// <returns>True if the user is authenticated.</returns>
    Task<bool> IsAuthenticatedAsync();
}
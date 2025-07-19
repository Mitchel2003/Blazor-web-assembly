using AppWeb.Shared.Services.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;
using AppWeb.Client.Auth;
using System.Web;

namespace AppWeb.Client.Services;

/// <summary>Provides navigation capabilities using NavigationManager.</summary>
public class BlazorNavigationService : INavigationService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly NavigationManager _navigationManager;
    private readonly JwtAuthStateProvider _authStateProvider;

    public BlazorNavigationService(NavigationManager navigationManager, IJSRuntime jsRuntime, JwtAuthStateProvider authStateProvider)
    {
        _jsRuntime = jsRuntime;
        _navigationManager = navigationManager;
        _authStateProvider = authStateProvider;
    }

    /// <summary>Simplified navigation to a route with default settings.</summary>
    public Task NavigateToAsync(string route) => NavigateToAsync(new NavigationConfig(route));

    /// <summary>Checks if the user is authenticated.</summary>
    public async Task<bool> IsAuthenticatedAsync() => await _authStateProvider.IsAuthenticatedAsync();

    /// <summary>Gets a query parameter from the current URL.</summary>
    public string? GetQueryParam(string paramName)
    {
        var uri = new Uri(_navigationManager.Uri);
        var query = HttpUtility.ParseQueryString(uri.Query);
        return query[paramName];
    }

    /// <summary>Navigates to a route using a NavigationConfig object.</summary>
    public async Task NavigateToAsync(NavigationConfig config)
    {
        try
        { //start with the base route
            string url = config.Route;
            if (config.Parameters.Count > 0)
            { //add query parameters if any
                var queryString = string.Join("&", config.Parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value?.ToString() ?? string.Empty)}"));
                url = $"{url}?{queryString}";
            }
            _navigationManager.NavigateTo(url, config.ForceReload, config.ReplaceHistory);
            await Task.Delay(50); //allow for a short delay to ensure navigation is processed
        }
        catch (Exception ex) { Debug.WriteLine($"Navigation error: {ex.Message}"); throw; }
    }
}
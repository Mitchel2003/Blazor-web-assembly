using AppWeb.SharedClient.Services.Adapters;
using AppWeb.Shared.Services.Contracts;

namespace AppWeb.Maui.Services;

/// <summary>
/// MAUI implementation of the INavigationService defined in the ViewModels project.
/// Provides navigation capabilities for the MAUI application.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly Dictionary<string, Type> _pageTypes = new();
    private readonly AuthService _authService;

    public NavigationService(AuthService authService)
    { _authService = authService; }

    /// <summary>Registers a page type for a specific route.</summary>
    public void RegisterPageType(string route, Type pageType)
    {
        if (!_pageTypes.ContainsKey(route))
        { _pageTypes.Add(route, pageType); }
    }

    /// <summary>Navigates to the specified route with configuration options.</summary>
    public async Task NavigateToAsync(NavigationConfig config)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));
        if (!_pageTypes.TryGetValue(config.Route, out Type pageType)) throw new InvalidOperationException($"No page registered for route: {config.Route}");

        // Create page instance
        var page = Activator.CreateInstance(pageType) as Page;
        if (page == null) throw new InvalidOperationException($"Failed to create page of type {pageType.Name}");

        // Pass parameters to page's BindingContext if it has OnNavigatingToAsync method
        if (config.Parameters?.Count > 0 && page.BindingContext != null)
        {
            var viewModel = page.BindingContext;
            var method = viewModel.GetType().GetMethod("OnNavigatingToAsync");
            if (method != null) await (Task)method.Invoke(viewModel, new[] { config.Parameters });
        }

        // Perform navigation based on config
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (config.ReplaceHistory)
            {
                // Replace current page
                if (Application.Current.MainPage is NavigationPage navPage)
                {
                    var currentStack = navPage.Navigation.NavigationStack;
                    if (currentStack.Count > 0)
                    {
                        navPage.Navigation.InsertPageBefore(page, currentStack[currentStack.Count - 1]);
                        await navPage.Navigation.PopAsync(config.ForceReload);
                        return;
                    }
                }

                //No navigation stack or empty stack, just set as main page
                Application.Current.MainPage = new NavigationPage(page);
            }
            else
            {
                // Add page to navigation stack
                if (Application.Current.MainPage is NavigationPage navPage)
                { await navPage.Navigation.PushAsync(page, config.ForceReload); }
                else { Application.Current.MainPage = new NavigationPage(page); }
            }
        });
    }

    /// <summary>Navigates to the specified route.</summary>
    public async Task NavigateToAsync(string route)
    { await NavigateToAsync(new NavigationConfig(route)); }

    /// <summary>Gets a query parameter from the current route.</summary>
    public string? GetQueryParam(string paramName)
    {
        // MAUI doesn't have a direct equivalent to query params in URL
        // We'll need to get this from a shared state or the app's last navigation parameters
        // For now, return null
        return null;
    }

    /// <summary>Checks if the user is authenticated.</summary>
    public async Task<bool> IsAuthenticatedAsync()
    { return await _authService.IsAuthenticatedAsync(); }

    // Additional MAUI-specific navigation methods

    /// <summary>Navigates back to the previous page.</summary>
    public async Task NavigateBackAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (Application.Current.MainPage is NavigationPage navigationPage)
            { await navigationPage.Navigation.PopAsync(); }
        });
    }

    /// <summary>Navigates to the root page.</summary>
    public async Task NavigateToRootAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (Application.Current.MainPage is NavigationPage navigationPage)
            { await navigationPage.Navigation.PopToRootAsync(); }
        });
    }
}
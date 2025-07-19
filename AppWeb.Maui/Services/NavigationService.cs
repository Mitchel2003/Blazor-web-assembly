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

    /// <summary>Checks if the user is authenticated.</summary>
    public async Task<bool> IsAuthenticatedAsync()
    { return await _authService.IsAuthenticatedAsync(); }

    /// <summary>Registers a page type for a specific route.</summary>
    public void RegisterPageType(string route, Type pageType)
    { if (!_pageTypes.ContainsKey(route)) _pageTypes.Add(route, pageType); }

    /// <summary>Gets a query parameter from the current route.</summary>
    public string? GetQueryParam(string paramName) { return null; }

    /// <summary>Navigates to the specified route.</summary>
    public async Task NavigateToAsync(string route)
    { await NavigateToMauiConfigAsync(new NavigationConfig(route)); }

    /// <summary>Navigates to the specified route with configuration options.</summary>
    public async Task NavigateToAsync(Shared.Services.Contracts.NavigationConfig config)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));
        var mauiConfig = new NavigationConfig(config.Route)
        { //build the maui navigation, using the shared config
            ReplaceHistory = config.ReplaceHistory,
            ForceReload = config.ForceReload
        };

        foreach (var param in config.Parameters)
        { mauiConfig.Parameters[param.Key] = param.Value; }
        await NavigateToMauiConfigAsync(mauiConfig);
    }

    /// <summary>Internal method to navigate with MAUI-specific configuration.</summary>
    private async Task NavigateToMauiConfigAsync(NavigationConfig config)
    {
        if (!_pageTypes.TryGetValue(config.Route, out Type pageType)) throw new InvalidOperationException($"No page registered for route: {config.Route}");
        var page = Activator.CreateInstance(pageType) as Page; //to create the page instance to navigate to
        if (page == null) throw new InvalidOperationException($"Failed to create page of type {pageType.Name}");

        //Pass parameters to page's BindingContext if it has OnNavigatingToAsync method
        if (config.Parameters?.Count > 0 && page.BindingContext != null)
        {
            var viewModel = page.BindingContext;
            var method = viewModel.GetType().GetMethod("OnNavigatingToAsync");
            if (method != null) await (Task)method.Invoke(viewModel, new[] { config.Parameters });
        }

        await MainThread.InvokeOnMainThreadAsync(async () =>
        { //Perform navigation based on config
            if (config.ReplaceHistory)
            { //Replace current page
                if (Application.Current.MainPage is NavigationPage navPage)
                {
                    var currentStack = navPage.Navigation.NavigationStack;
                    if (currentStack.Count == 0) return; //No pages to replace
                    navPage.Navigation.InsertPageBefore(page, currentStack[currentStack.Count - 1]);
                    await navPage.Navigation.PopAsync(config.ForceReload);
                    return;
                }
                //No navigation stack or empty stack, just set as main page
                Application.Current.MainPage = new NavigationPage(page);
            }
            else
            {
                if (Application.Current.MainPage is NavigationPage navPage)
                { await navPage.Navigation.PushAsync(page, config.ForceReload); }
                else { Application.Current.MainPage = new NavigationPage(page); }
            }
        });
    }

    /// <summary>Navigates back to the previous page.</summary>
    public async Task NavigateBackAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        { if (Application.Current.MainPage is NavigationPage navigationPage) await navigationPage.Navigation.PopAsync(); });
    }

    /// <summary>Navigates to the root page.</summary>
    public async Task NavigateToRootAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        { if (Application.Current.MainPage is NavigationPage navigationPage) await navigationPage.Navigation.PopToRootAsync(); });
    }
}

#region Interface config ------------------------------------------------------------
public class NavigationConfig
{
    /// <summary>Known routes of the application for easy access.</summary>
    public static class Routes
    {
        public const string Home = "/";
        public const string Login = "login";
        public const string Logout = "logout";
        public const string Users = "users";
        public const string CreateUser = "users/create";
        public const string EditUser = "users/edit";
    }

    /// <summary>Constructor with route and parameters.</summary>
    public NavigationConfig(string route, Dictionary<string, object>? parameters)
    {
        Route = route;
        ForceReload = false;
        ReplaceHistory = false;
        Parameters = parameters ?? new Dictionary<string, object>();
    }

    /// <summary>Constructor with only route.</summary>
    public NavigationConfig(string route) : this(route, null) { }

    /// <summary>The route to navigate to.</summary>
    public string Route { get; }

    /// <summary>Whether to force reload the page.</summary>
    public bool ForceReload { get; set; }

    /// <summary>Whether to replace the navigation history.</summary>
    public bool ReplaceHistory { get; set; }

    /// <summary>Dictionary of navigation parameters.</summary>
    public Dictionary<string, object> Parameters { get; }

    /// <summary>Create a navigation configuration with replace history option.</summary>
    public static NavigationConfig CreateWithReplaceHistory(string route, Dictionary<string, object>? parameters = null)
    { return new NavigationConfig(route, parameters) { ReplaceHistory = true }; }
}
#endregion ---------------------------------------------------------------------
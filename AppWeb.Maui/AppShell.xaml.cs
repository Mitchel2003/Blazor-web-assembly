using AppWeb.Shared.Services.Contracts;
using AppWeb.Maui.Views.Users;
using AppWeb.Maui.Views.Auth;
using AppWeb.Maui.Views;

namespace AppWeb.Maui;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    public AppShell(IAuthService authService, INavigationService navigationService)
    {
        InitializeComponent();

        _authService = authService;
        _navigationService = navigationService;

        // Register routes for Shell navigation
        RegisterRoutes();

        // Subscribe to authentication state changes
        var mauiAuthService = authService;
        if (mauiAuthService != null)
        {
            mauiAuthService.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }
    }

    private void RegisterRoutes()
    {
        // Register Shell routes for navigation
        Routing.RegisterRoute(NavigationConfig.Routes.Home, typeof(HomePage));
        Routing.RegisterRoute(NavigationConfig.Routes.Login, typeof(LoginPage));
        Routing.RegisterRoute(NavigationConfig.Routes.Users, typeof(TableUsersPage));
        Routing.RegisterRoute(NavigationConfig.Routes.CreateUser, typeof(UserFormPage));
        Routing.RegisterRoute(NavigationConfig.Routes.EditUser, typeof(UserFormPage));
        // Add more routes as needed
    }

    private async void OnAuthenticationStateChanged(object sender, object args)
    {
        // Update UI based on authentication state
        bool isAuthenticated = await _authService.IsAuthenticatedAsync();
        if (!isAuthenticated)
        { //If logged out, navigate to login page
            await _navigationService.NavigateToAsync(NavigationConfig.Routes.Login);
        }
    }
}

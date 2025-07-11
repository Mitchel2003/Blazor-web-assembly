using AppWeb.Shared.Services.Contracts;
using AppWeb.Maui.Services;

namespace AppWeb.Maui;

public partial class App : Application
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly NavigationService _mauiNavigationService;
    private readonly Action<NavigationService> _routeConfiguration;

    public App(IAuthService authService, INavigationService navigationService, Action<NavigationService> routeConfiguration)
    {
        InitializeComponent();
        _authService = authService;
        _navigationService = navigationService;
        _routeConfiguration = routeConfiguration;
        _mauiNavigationService = navigationService as NavigationService;

        //Configure routes in navigation service
        _routeConfiguration(_mauiNavigationService);

        //Initialize MainPage to AppShell
        MainPage = new AppShell(authService, navigationService);
        //Check for authentication and navigate accordingly
        Startup();
    }

    private async void Startup()
    {
        try
        { //Check if user is authenticated and navigate accordingly
            bool isAuthenticated = await _authService.IsAuthenticatedAsync();
            if (isAuthenticated) { await _navigationService.NavigateToAsync(NavigationConfig.Routes.Home); }
            else { await _navigationService.NavigateToAsync(NavigationConfig.Routes.Login); }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Startup navigation failed: {ex.Message}");
            await _navigationService.NavigateToAsync(NavigationConfig.Routes.Login);
        }
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        //Set window size and other properties if needed
        window.Width = 800;
        window.Height = 600;
        window.Title = "AppWeb MAUI";

        return window;
    }
}
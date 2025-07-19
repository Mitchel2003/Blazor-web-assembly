using AppWeb.Shared.Services.Contracts;
using System.Diagnostics;

namespace AppWeb.Maui;

public partial class AppShell : Shell
{
    private readonly IAuthService? _authService;
    private readonly INavigationService? _navigationService;

    //Constructor for use from XAML (necessary for initialization)
    public AppShell()
    {
        try { InitializeComponent(); }
        catch (Exception ex) { Debug.WriteLine($"Error al inicializar AppShell: {ex.Message}"); }
    }

    //Constructor for use with dependency injection
    public AppShell(IAuthService authService, INavigationService navigationService)
    {
        try
        {
            InitializeComponent();
            _authService = authService;
            _navigationService = navigationService;

            //Configure navigation routes
            RegisterRoutes();

            //Start navigation
            MainThread.BeginInvokeOnMainThread(async () => await StartupNavigation());
        }
        catch (Exception ex) { Debug.WriteLine($"Error initializing AppShell with services: {ex.Message}"); }
    }

    private void RegisterRoutes()
    {
        try
        { //Register navigation routes
            Routing.RegisterRoute(Services.NavigationConfig.Routes.Home, typeof(Views.HomePage));
            Routing.RegisterRoute(Services.NavigationConfig.Routes.Login, typeof(Views.Auth.LoginPage));
            Routing.RegisterRoute(Services.NavigationConfig.Routes.Users, typeof(Views.Users.TableUsersPage));
            Routing.RegisterRoute(Services.NavigationConfig.Routes.CreateUser, typeof(Views.Users.UserFormPage));
            Routing.RegisterRoute(Services.NavigationConfig.Routes.EditUser, typeof(Views.Users.UserFormPage));
        }
        catch (Exception ex) { Debug.WriteLine($"Error registering routes: {ex.Message}"); }
    }

    private async Task StartupNavigation()
    {
        if (_authService != null && _navigationService != null)
        { //Check authentication and navigate to the corresponding page
            bool isAuthenticated = await _authService.IsAuthenticatedAsync();
            if (isAuthenticated) { await _navigationService.NavigateToAsync(Services.NavigationConfig.Routes.Home); }
            else { await _navigationService.NavigateToAsync(Services.NavigationConfig.Routes.Login); }
        }
        else { Debug.WriteLine("Navigation could not be started: services not available"); }
    }
}
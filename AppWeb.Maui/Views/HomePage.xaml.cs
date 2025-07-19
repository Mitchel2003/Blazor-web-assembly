using AppWeb.Shared.Services.Contracts;

namespace AppWeb.Maui.Views;

public partial class HomePage : ContentPage
{
    private readonly INavigationService _navigationService;

    public HomePage(INavigationService navigationService)
    {
        InitializeComponent();
        _navigationService = navigationService;
        //Set up command binding for the navigation command
        BindingContext = this;
    }
    
    //Command to navigate to the users page
    public Command NavigateToUsersCommand => new Command(async () => 
    { await _navigationService.NavigateToAsync(NavigationConfig.Routes.Users); });
}
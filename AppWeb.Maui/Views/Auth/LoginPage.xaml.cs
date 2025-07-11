using AppWeb.ViewModels.Features.Contracts;
using AppWeb.ViewModels.Features.Auth;
using AppWeb.Maui.Views.Common;

namespace AppWeb.Maui.Views.Auth;

public partial class LoginPage : BaseContentPage<ILoginVM>
{
    public LoginPage(ILoginVM loginViewModel) : base(loginViewModel)
    {
        InitializeComponent();
        // Subscribe to events from the ViewModel
        if (ViewModel is LoginVM concreteViewModel) concreteViewModel.LoginCompleted += OnLoginCompleted;
    }

    private async void OnLoginCompleted(object sender, LoginSuccessEventArgs e)
    { //Refresh the UI when login completes, UI updates can be performed here after login
        MainThread.BeginInvokeOnMainThread(() => { });
    }

    protected override void OnDisappearing()
    { //Unsubscribe from events to prevent memory leaks
        if (ViewModel is LoginVM concreteViewModel) concreteViewModel.LoginCompleted -= OnLoginCompleted;
        base.OnDisappearing();
    }
}
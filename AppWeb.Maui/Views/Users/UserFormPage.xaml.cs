using AppWeb.ViewModels.Features.Contracts;
using AppWeb.ViewModels.Features.Users;
using AppWeb.Maui.Views.Common;

namespace AppWeb.Maui.Views.Users;

public partial class UserFormPage : BaseContentPage<ICreateUserVM>
{
    public UserFormPage(ICreateUserVM viewModel) : base(viewModel)
    {
        InitializeComponent();
        //Subscribe to ViewModel events if it's the concrete type
        if (ViewModel is CreateUserVM concreteViewModel) concreteViewModel.UserCreated += OnUserCreated;
    }
    
    protected override Task OnViewModelAppearing()
    { return base.OnViewModelAppearing(); }

    private void OnUserCreated(object sender, UserCreatedEventArgs e)
    { if (e.Success) MainThread.BeginInvokeOnMainThread(() => { }); } //UI updates can be performed here after user is created

    protected override void OnDisappearing()
    { //Unsubscribe from events to prevent memory leaks
        if (ViewModel is CreateUserVM concreteViewModel) concreteViewModel.UserCreated -= OnUserCreated;
        base.OnDisappearing();
    }
}
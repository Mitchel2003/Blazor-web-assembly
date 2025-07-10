using AppWeb.ViewModels.Features.Contracts;
using CommunityToolkit.Mvvm.ComponentModel;
using AppWeb.Shared.Services.Contracts;
using AppWeb.ViewModels.Core.Factory;
using CommunityToolkit.Mvvm.Input;
using AppWeb.ViewModels.Core.Base;
using AppWeb.Shared.Inputs;

namespace AppWeb.ViewModels.Features.Users;

/// <summary>ViewModel for creating new users.</summary>
public partial class CreateUserVM : ViewModelCrud<CreateUserInput, int>, ICreateUserVM
{
    private readonly IUsersService _usersService;
    private readonly IMessageService _messageService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private bool _showPassword;
    [ObservableProperty] private bool _redirectionInProgress;
    public event EventHandler<UserCreatedEventArgs>? UserCreated;

    public CreateUserVM(IUsersService usersService, IMessageService messageService, INavigationService navigationService, IModelFactory modelFactory)
        : base(modelFactory, messageService)
    {
        Title = "Create User";
        _usersService = usersService;
        _messageService = messageService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    /// <summary>Toggles password visibility.</summary>
    private async Task TogglePasswordVisibilityAsync()
    { ShowPassword = !ShowPassword; await Task.CompletedTask; }

    /// <summary>Initializes the ViewModel with default values.</summary>
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    { //reset success state on initialization
        OperationSuccess = false;
        RedirectionInProgress = false;
        await base.InitializeAsync(cancellationToken);
        IsNew = true; //ensure this is marked as a new entity
        Model.IsActive = true; //set default values
    }

    /// <summary>Implementation for saving a new user.</summary>
    protected override async Task<bool> OnSaveAsync()
    {
        try
        {
            var result = await _usersService.CreateUserAsync(Model);
            if (result == null) { await _messageService.ShowErrorAsync("Error creating user"); return false; }
            await _messageService.ShowSuccessAsync("User created successfully");
            ModelId = result.Id; //set the model ID from the result, if applicable
            UserCreated?.Invoke(this, new UserCreatedEventArgs //notify subscribers
            { Success = true, UserId = result.Id, Username = result.Username ?? Model.Username });

            RedirectionInProgress = true; //initiate redirection
            await Task.Delay(800); //to allow the success message
            //verify if the user is authenticated, if not, navigate to the login
            var isAuthenticated = await _navigationService.IsAuthenticatedAsync();
            if (isAuthenticated) await NavigateToUsersAsync();
            else await NavigateToLoginAsync();
            return true;
        }
        catch (Exception ex) { await _messageService.ShowErrorAsync($"Unexpected error: {ex.Message}"); return false; }
    }

    [RelayCommand]
    /// <summary>Navigate back with confirmation if changes are pending.</summary>
    public async Task NavigateBackAsync()
    {
        if (RedirectionInProgress) return;
        if (IsModified)
        { //check for unsaved changes
            var confirm = await _messageService.ConfirmAsync("Discard Changes", "You have unsaved changes. Are you sure you want to discard them?");
            if (!confirm) return;
        }

        RedirectionInProgress = true;
        //notify subscribers about navigation
        UserCreated?.Invoke(this, new UserCreatedEventArgs
        { Success = false, ShouldNavigate = true });
        //verify if the user is authenticated, handle the navigation
        var isAuthenticated = await _navigationService.IsAuthenticatedAsync();
        //navigate to the users list or the login page, if not authenticated
        if (isAuthenticated) await NavigateToUsersAsync();
        else await NavigateToLoginAsync();
    }

    #region Helpers ------------------------------------------------------------
    /// <summary>Navigate to the users list.</summary>
    private async Task NavigateToUsersAsync()
    { await _navigationService.NavigateToAsync(new NavigationConfig(NavigationConfig.Routes.Users)); }

    /// <summary>Navigate to the login page.</summary>
    private async Task NavigateToLoginAsync()
    { await _navigationService.NavigateToAsync(new NavigationConfig(NavigationConfig.Routes.Login)); }

    /// <summary>Required by ViewModelCrud but not used for Create operations</summary>
    public override Task<CreateUserInput> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    { return Task.FromResult(_modelFactory.Create<CreateUserInput>()); } //Not applicable for creation, but required by base class

    /// <summary>Shows validation errors as a formatted message.</summary>
    private async void ShowMessageErrorValidation()
    {
        var errors = GetAllErrors(); //get validation error in context
        if (errors.Count > 0) await _messageService.ShowErrorAsync(string.Join("\n", errors));
    }
    #endregion ---------------------------------------------------------------------
}
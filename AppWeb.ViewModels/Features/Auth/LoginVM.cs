using CommunityToolkit.Mvvm.ComponentModel;
using AppWeb.ViewModels.Features.Contracts;
using AppWeb.Shared.Services.Contracts;
using AppWeb.ViewModels.Core.Factory;
using CommunityToolkit.Mvvm.Input;
using AppWeb.ViewModels.Core.Base;
using AppWeb.Shared.Inputs;

namespace AppWeb.ViewModels.Features.Auth;

/// <summary>ViewModel for login functionality with validation.</summary>
public partial class LoginVM : ViewModelCrud<LoginInput, int>, ILoginVM
{
    private readonly IAuthService _authService;
    private readonly IMessageService _messageService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private bool _showPassword;
    [ObservableProperty] private bool _redirectionInProgress;
    public event EventHandler<LoginSuccessEventArgs>? LoginCompleted;

    public LoginVM(IAuthService authService, IMessageService messageService, INavigationService navigationService, IModelFactory modelFactory)
        : base(modelFactory)
    {
        Title = "Login";
        _authService = authService;
        _messageService = messageService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    /// <summary>Toggles password visibility.</summary>
    private async Task TogglePasswordVisibilityAsync()
    { ShowPassword = !ShowPassword; await Task.CompletedTask; }

    /// <summary>Checks if authenticated users should be redirected.</summary>
    public async Task<bool> ShouldRedirectAuthenticatedUserAsync()
    { return await _authService.IsAuthenticatedAsync(); }

    /// <summary>Initializes the ViewModel, reset the login success state on initialization</summary>
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        OperationSuccess = false;
        RedirectionInProgress = false;
        await base.InitializeAsync(cancellationToken);
    }

    [RelayCommand]
    /// <summary>Handles the login process with validation.</summary>
    private async Task LoginAsync(object? formObject)
    {
        if (formObject == null) { await _messageService.ShowErrorAsync("Form validation failed"); return; }
        if (!await ValidateAsync()) { ShowMessageErrorValidation(); return; }
        try
        {
            IsSaving = true;
            var loginResult = await _authService.LoginAsync(Model);
            if (loginResult == null) { await _messageService.ShowErrorAsync("Login failed"); return; }
            OperationSuccess = true; // Set login success state

            //Notify subscribers about completed login
            LoginCompleted?.Invoke(this, new LoginSuccessEventArgs
            {
                Token = loginResult.Token,
                Username = loginResult.Username ?? Model.Email,
                UserId = loginResult.UserId,
                ShouldNavigate = true
            });

            RedirectionInProgress = true; //Initiate redirection
            await Task.Delay(1000); //To allow the success message
            await NavigateAfterLoginAsync(); //Navigate based on return URL or to home
        }
        catch (Exception ex) { await _messageService.ShowErrorAsync($"Error during login: {ex.Message}"); }
        finally { IsSaving = false; }
    }

    /// <summary>Navigate to appropriate page after login success.</summary>
    private async Task NavigateAfterLoginAsync()
    {
        var returnUrl = _navigationService.GetQueryParam("returnUrl");
        if (!string.IsNullOrEmpty(returnUrl))
        { //to avoid base path issues, ensure no leading slash
            if (returnUrl.StartsWith("/")) returnUrl = returnUrl.Substring(1);
            await _navigationService.NavigateToAsync(returnUrl);
        } //navigate to home with force reload for fresh state
        else { await _navigationService.NavigateToAsync(new NavigationConfig(NavigationConfig.Routes.Home)); }
    }

    #region Helpers ------------------------------------------------------------
    /// <summary>Required by ViewModelCrud but not used for Login</summary>
    public override Task<LoginInput> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    { return Task.FromResult(_modelFactory.Create<LoginInput>()); }

    /// <summary>Override the default save behavior since login has special handling</summary>
    protected override Task<bool> OnSaveAsync() { return Task.FromResult(false); }

    /// <summary>Shows validation errors as a formatted message</summary>
    private async void ShowMessageErrorValidation()
    {
        var errors = GetAllErrors(); //get validation error in context
        if (errors.Count > 0) await _messageService.ShowErrorAsync(string.Join("\n", errors));
    }
    #endregion ---------------------------------------------------------------------
}
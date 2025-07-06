using CommunityToolkit.Mvvm.ComponentModel;
using AppWeb.ViewModels.Core.Services;
using CommunityToolkit.Mvvm.Input;
using AppWeb.ViewModels.Core.Base;

namespace AppWeb.ViewModels.Features.Auth;

/// <summary>ViewModel for logout functionality.</summary>
public partial class LogoutVM : ViewModelBase, ILogoutVM
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private bool _isLoggingOut;
    [ObservableProperty] private bool _logoutCompleted;
    [ObservableProperty] private string _errorMessage = string.Empty;
    public event EventHandler<LogoutCompletedEventArgs>? LogoutCompletedEvent;

    public LogoutVM(IAuthService authService, INavigationService navigationService)
    {
        Title = "Logout";
        _authService = authService;
        _navigationService = navigationService;
    }

    /// <summary>Initialize the ViewModel.</summary>
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await base.InitializeAsync(cancellationToken);
        LogoutCompleted = false; //Reset state init
        await LogoutCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    /// <summary>Perform logout and notify subscribers.</summary>
    private async Task LogoutAsync()
    {
        if (IsLoggingOut) return;
        try
        {
            IsLoggingOut = true;
            await _authService.LogoutAsync();
            LogoutCompleted = true; // Set the flag to indicate logout is complete
            LogoutCompletedEvent?.Invoke(this, new LogoutCompletedEventArgs { Success = true });
            await Task.Delay(500); //Give time for UI to update before redirect, ensure logout is processed
            await _navigationService.NavigateToAsync(new NavigationConfig(NavigationConfig.Routes.Login));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error durante el cierre de sesión: {ex.Message}";
            LogoutCompletedEvent?.Invoke(this, new LogoutCompletedEventArgs { Success = false, ErrorMessage = ErrorMessage });
        }
        finally { IsLoggingOut = false; }
    }
}
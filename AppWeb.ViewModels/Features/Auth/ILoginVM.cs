using CommunityToolkit.Mvvm.Input;
using AppWeb.ViewModels.Core.Base;
using AppWeb.Shared.Inputs;

namespace AppWeb.ViewModels.Features.Auth;

/// <summary>Interface for the LoginVM.</summary>
public interface ILoginVM : IViewModelCrud<LoginInput, int>
{
    /// <summary>Indicates if the password should be shown in plain text.</summary>
    bool ShowPassword { get; }

    /// <summary>Indicates if a redirection is in progress.</summary>
    bool RedirectionInProgress { get; }

    /// <summary>Command to execute the login process.</summary>
    IAsyncRelayCommand<object> LoginCommand { get; }

    /// <summary>Command to toggle password visibility.</summary>
    IAsyncRelayCommand TogglePasswordVisibilityCommand { get; }

    /// <summary>Event fired when login is successful.</summary>
    event EventHandler<LoginSuccessEventArgs> LoginSuccessful;

    /// <summary>Determines if authenticated users should be redirected.</summary>
    Task<bool> ShouldRedirectAuthenticatedUserAsync();

    /// <summary>Initializes the ViewModel.</summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
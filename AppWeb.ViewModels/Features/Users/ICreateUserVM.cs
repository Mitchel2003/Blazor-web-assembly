using CommunityToolkit.Mvvm.Input;
using AppWeb.ViewModels.Core.Base;
using AppWeb.Shared.Inputs;

namespace AppWeb.ViewModels.Features.Users;

/// <summary>Interface for the CreateUserVM.</summary>
public interface ICreateUserVM : IViewModelCrud<CreateUserInput, int>
{
    /// <summary>Indicates if the password should be shown.</summary>
    bool ShowPassword { get; }

    /// <summary>Indicates if redirection is in progress.</summary>
    bool RedirectionInProgress { get; }

    /// <summary>Command to navigate back.</summary>
    IAsyncRelayCommand NavigateBackCommand { get; }

    /// <summary>Command to toggle password visibility.</summary>
    IAsyncRelayCommand TogglePasswordVisibilityCommand { get; }

    /// <summary>Event fired when a user is created.</summary>
    event EventHandler<UserCreatedEventArgs> UserCreated;

    /// <summary>Initializes the ViewModel.</summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
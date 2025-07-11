using AppWeb.ViewModels.Features.Auth;
using CommunityToolkit.Mvvm.Input;

namespace AppWeb.ViewModels.Features.Contracts;

public interface ILogoutVM
{
    /// <summary>Error message if logout failed.</summary>
    string ErrorMessage { get; }

    /// <summary>Indicates if logout is in progress.</summary>
    bool IsLoggingOut { get; }

    /// <summary>Indicates if logout has completed successfully.</summary>
    bool LogoutCompleted { get; }

    /// <summary>Command to perform logout.</summary>
    IAsyncRelayCommand LogoutCommand { get; }

    /// <summary>Event fired when logout is completed.</summary>
    event EventHandler<LogoutCompletedEventArgs> LogoutCompletedEvent;

    /// <summary>Initializes the ViewModel.</summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
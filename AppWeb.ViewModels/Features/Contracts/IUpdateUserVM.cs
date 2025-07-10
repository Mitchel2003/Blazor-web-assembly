using AppWeb.ViewModels.Features.Users;
using AppWeb.ViewModels.Core.Base;
using CommunityToolkit.Mvvm.Input;
using AppWeb.Shared.Inputs;

namespace AppWeb.ViewModels.Features.Contracts;

/// <summary>Interface for the UpdateUserVM.</summary>
public interface IUpdateUserVM : IViewModelCrud<UpdateUserInput, int>
{
    /// <summary>Indicates if redirection is in progress.</summary>
    bool RedirectionInProgress { get; }
    
    /// <summary>Command to navigate back.</summary>
    IAsyncRelayCommand NavigateBackCommand { get; }
    
    /// <summary>Event fired when a user is updated.</summary>
    event EventHandler<UserUpdatedEventArgs> UserUpdated;
}
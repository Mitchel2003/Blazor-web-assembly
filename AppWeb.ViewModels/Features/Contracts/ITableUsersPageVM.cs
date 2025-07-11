using AppWeb.ViewModels.Core.Base;
using CommunityToolkit.Mvvm.Input;
using AppWeb.Shared.Dtos;

namespace AppWeb.ViewModels.Features.Contracts;

/// <summary>Interface for the TableUsersPageVM.</summary>
public interface ITableUsersPageVM : IViewModelBase
{
    /// <summary>Indicates if the page is currently loading data.</summary>
    bool IsLoading { get; }
    
    /// <summary>List of users to display in the page.</summary>
    IReadOnlyList<UserResultDto>? Users { get; }
    
    /// <summary>Command to add a new user.</summary>
    IAsyncRelayCommand AddUserCommand { get; }
    
    /// <summary>Command to edit a user.</summary>
    IAsyncRelayCommand<UserResultDto> EditUserCommand { get; }
    
    /// <summary>Command to delete a user.</summary>
    IAsyncRelayCommand<UserResultDto> DeleteUserCommand { get; }
    
    /// <summary>Command to view user details.</summary>
    IAsyncRelayCommand<UserResultDto> ViewUserCommand { get; }
    
    /// <summary>Command to refresh the user list.</summary>
    IAsyncRelayCommand LoadUsersCommand { get; }
    
    /// <summary>Command to delete multiple users.</summary>
    IAsyncRelayCommand<List<UserResultDto>> BulkDeleteUsersCommand { get; }
    
    /// <summary>Event raised when a user is selected.</summary>
    event EventHandler<UserResultDto>? UserSelected;
    
    /// <summary>Event raised when a user is deleted.</summary>
    event EventHandler<UserResultDto>? UserDeleted;
    
    /// <summary>Loads the users data.</summary>
    Task LoadAsync();
} 
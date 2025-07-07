using CommunityToolkit.Mvvm.Input;
using AppWeb.Shared.Dtos;

namespace AppWeb.ViewModels.Features.Users;

/// <summary>Interface for the TableUsersPageVM.</summary>
public interface ITableUsersPageVM : IDisposable
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
    IAsyncRelayCommand RefreshCommand { get; }
    
    /// <summary>Command to delete multiple users.</summary>
    IAsyncRelayCommand<List<UserResultDto>> BulkDeleteCommand { get; }
    
    /// <summary>Loads the users data.</summary>
    Task LoadAsync();
} 
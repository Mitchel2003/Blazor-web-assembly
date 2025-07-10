using AppWeb.ViewModels.Features.Contracts;
using CommunityToolkit.Mvvm.ComponentModel;
using AppWeb.Shared.Services.Contracts;
using AppWeb.ViewModels.Core.Base;
using CommunityToolkit.Mvvm.Input;
using AppWeb.Shared.Dtos;

namespace AppWeb.ViewModels.Features.Users;

/// <summary>ViewModel for the users table page.</summary>
public partial class TableUsersPageVM : ViewModelBase, ITableUsersPageVM
{
    private readonly IUsersService _usersService;
    private readonly IMessageService _messageService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private IReadOnlyList<UserResultDto>? _users;
    [ObservableProperty] private bool _isLoading;

    public TableUsersPageVM(IUsersService usersService, IMessageService messageService, INavigationService navigationService)
    {
        _usersService = usersService;
        _messageService = messageService;
        _navigationService = navigationService;
        Title = "Users";
    }

    /// <summary>Loads users from the service.</summary>
    public async Task LoadAsync()
    {
        try { IsLoading = true; Users = await _usersService.GetUsersAsync(); }
        catch (Exception ex) { ErrorMessage = $"Error loading users: {ex.Message}"; await _messageService.ShowErrorAsync(ErrorMessage); }
        finally { IsLoading = false; }
    }

    /// <summary>Initializes the view model.</summary>
    protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
    { await LoadAsync(); }

    [RelayCommand]
    /// <summary>Handles adding a new user.</summary>
    private async Task AddUserAsync()
    { await _navigationService.NavigateToAsync(new NavigationConfig(NavigationConfig.Routes.CreateUser)); }

    [RelayCommand]
    /// <summary>Handles editing a user.</summary>
    private async Task EditUserAsync(UserResultDto user)
    {
        var config = new NavigationConfig($"{NavigationConfig.Routes.EditUser}/{user.Id}");
        await _navigationService.NavigateToAsync(config);
    }

    [RelayCommand]
    /// <summary>Handles viewing user details.</summary>
    private async Task ViewUserAsync(UserResultDto user)
    {
        IsLoading = true;
        var fullUser = await _usersService.GetUserByIdAsync(user.Id);
        if (fullUser == null) { await _messageService.ShowErrorAsync($"User #{user.Id} not found."); return; }

        // Here you would typically open a dialog or navigate to details view
        // For now, just show a success message
        await _messageService.ShowSuccessAsync($"Viewed user: {fullUser.Username}");
        IsLoading = false;
    }

    [RelayCommand]
    /// <summary>Handles deleting a user.</summary>
    private async Task DeleteUserAsync(UserResultDto user)
    {
        var confirm = await _messageService.ConfirmAsync("Confirm Delete", $"Delete user '{user.Username}'?", "Delete", "Cancel");
        if (!confirm) return;
        try
        {
            IsLoading = true;
            var success = await _usersService.DeleteUserAsync(user.Id);
            if (success) { await LoadAsync(); await _messageService.ShowSuccessAsync($"User '{user.Username}' deleted successfully."); }
            else { await _messageService.ShowErrorAsync($"Failed to delete user '{user.Username}'."); }
        }
        catch (Exception ex) { await _messageService.ShowErrorAsync($"Error deleting user: {ex.Message}"); }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    /// <summary>Handles refreshing the user list.</summary>
    private async Task LoadUsersAsync()
    { await LoadAsync(); }

    [RelayCommand]
    /// <summary>Handles bulk deletion of users.</summary>
    private async Task BulkDeleteUsersAsync(List<UserResultDto> usersToDelete)
    {
        if (usersToDelete.Count == 0) return;
        var confirm = await _messageService.ConfirmAsync("Confirm Bulk Delete", $"Delete {usersToDelete.Count} selected users?", "Delete All", "Cancel");
        if (!confirm) return;
        try
        {
            IsLoading = true;
            int successCount = 0;
            List<string> errors = new();
            foreach (var user in usersToDelete)
            {
                var success = await _usersService.DeleteUserAsync(user.Id);
                if (success) successCount++; // Increment success count
                else errors.Add($"Failed to delete user {user.Username} (ID: {user.Id})");
            }
            await LoadAsync(); // Refresh the list
            if (successCount > 0) await _messageService.ShowSuccessAsync($"{successCount} users deleted successfully.");
            foreach (var error in errors) { await _messageService.ShowErrorAsync(error); }
        }
        catch (Exception ex) { await _messageService.ShowErrorAsync($"Bulk delete operation failed: {ex.Message}"); }
        finally { IsLoading = false; }
    }
}
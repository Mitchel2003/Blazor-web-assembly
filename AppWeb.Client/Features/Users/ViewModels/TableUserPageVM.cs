using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using AppWeb.Client.Services;
using AppWeb.Shared.Dtos;
using MudBlazor;

namespace AppWeb.Client.Features.Users.ViewModels;

/// <summary>
/// View-model for TableUserPage. Encapsulates UI state and orchestrates
/// communication with <see cref="IUsersApiClient"/>.
/// </summary>
public partial class TableUserPageVM : ObservableObject
{
    private readonly IDialogService _dialogService;
    private readonly IUsersApiClient _usersApi;
    private readonly NavigationManager _nav;
    private readonly ISnackbar _snackbar;

    #region Observable ------------------------------------------------------------
    [ObservableProperty] private IReadOnlyList<UserResultDto>? users;
    [ObservableProperty] private bool isLoading;
    #endregion ---------------------------------------------------------------------

    public TableUserPageVM(IUsersApiClient usersApi, IDialogService dialogService, ISnackbar snackbar, NavigationManager nav)
    {
        _usersApi = usersApi;
        _dialogService = dialogService;
        _snackbar = snackbar;
        _nav = nav;
    }

    /// <summary> Loads existing users for rendering on table </summary>
    public async Task LoadAsync()
    {
        try { IsLoading = true; Users = await _usersApi.GetUsersAsync(); }
        catch (Errors.ApiException apiEx) { foreach (var err in apiEx.Errors) _snackbar.Add(err, Severity.Error); }
        catch (Exception ex) { _snackbar.Add($"Error loading users: {ex.Message}", Severity.Error); }
        finally { IsLoading = false; }
    }

    /// <summary> Handles action user view coming from <see cref="Components.TableUser"/> </summary>
    public async Task HandleViewUser(UserResultDto user)
    {
        var parameters = new DialogParameters { ["Existing"] = user };
        await _dialogService.ShowAsync<Components.PreviewUser>($"User #{user.Id}", parameters);
    }

    /// <summary> Handles action user add coming from <see cref="Components.TableUser"/> </summary>
    public Task HandleAddUser()
    {
        _nav.NavigateTo("/users/create");
        return Task.CompletedTask;
    }

    /// <summary> Handles action user edit coming from <see cref="Components.TableUser"/> </summary>
    public Task HandleEditUser(UserResultDto user)
    {
        _nav.NavigateTo($"/users/edit/{user.Id}");
        return Task.CompletedTask;
    }

    /// <summary> Handles action user delete coming from <see cref="Components.TableUser"/>. </summary>
    public async Task HandleDeleteUser(UserResultDto user)
    {
        bool? confirm = await _dialogService.ShowMessageBox(
            "Confirm Delete", $"Delete user '{user.Username}'?",
            yesText: "Delete", cancelText: "Cancel", options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall }
        );
        try
        {
            IsLoading = true;
            if (confirm != true) return;
            var success = await _usersApi.DeleteUserAsync(user.Id); // Call the API to delete the user
            if (success is true) { await LoadAsync(); _snackbar.Add("User deleted", Severity.Info); }
        }
        catch (Errors.ApiException apiEx) { foreach (var err in apiEx.Errors) _snackbar.Add(err, Severity.Error); }
        catch (Exception ex) { _snackbar.Add($"Unexpected error: {ex.Message}", Severity.Error); }
        finally { IsLoading = false; }
    }

    /// <summary>Handles bulk deletion of multiple users</summary>
    public async Task HandleBulkDeleteUsers(List<UserResultDto> usersToDelete)
    {
        if (usersToDelete?.Count == 0) return;
        bool? confirm = await _dialogService.ShowMessageBox(
            "Confirm Bulk Delete", $"Delete {usersToDelete?.Count} selected users?",
            yesText: "Delete All", cancelText: "Cancel", options: new DialogOptions { MaxWidth = MaxWidth.Small }
        ); // Show confirmation dialog
        try
        {
            if (confirm != true) return;
            List<string> errors = new();
            IsLoading = true; //loading...
            int successCount = 0; //Count
            foreach (var user in usersToDelete!)
            {
                var success = await _usersApi.DeleteUserAsync(user.Id);
                if (success is true) { successCount++; } // Increment success count
                else { errors.Add($"Failed to delete user {user.Username} (ID: {user.Id})"); } // Collect errors
            }

            await LoadAsync();
            if (successCount > 0) { _snackbar.Add($"{successCount} users deleted successfully", Severity.Success); }
            foreach (var error in errors) { _snackbar.Add(error, Severity.Error); } // Show any errors encountered
        }
        catch (Errors.ApiException apiEx) { foreach (var err in apiEx.Errors) _snackbar.Add(err, Severity.Error); }
        catch (Exception ex) { _snackbar.Add($"Bulk delete operation failed: {ex.Message}", Severity.Error); }
        finally { IsLoading = false; }
    }
}
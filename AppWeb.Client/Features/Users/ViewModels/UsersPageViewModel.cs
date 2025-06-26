using AppWeb.Client.Features.Users.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using AppWeb.Client.Services;
using AppWeb.Shared.Dtos;
using MudBlazor;

namespace AppWeb.Client.Features.Users.ViewModels;

public partial class UsersPageViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    private readonly IUsersApiClient _usersApi;
    private readonly NavigationManager _nav;
    private readonly ISnackbar _snackbar;

    [ObservableProperty]
    private IReadOnlyList<UserResultDto>? users;
    public UsersPageViewModel(IUsersApiClient usersApi, IDialogService dialogService, ISnackbar snackbar, NavigationManager nav)
    {
        _usersApi = usersApi;
        _dialogService = dialogService;
        _snackbar = snackbar;
        _nav = nav;
    }

    public async Task LoadAsync() { Users = await _usersApi.GetUsersAsync(); }

    public async Task HandleViewUser(UserResultDto user)
    {
        var parameters = new DialogParameters { ["Existing"] = user };
        await _dialogService.ShowAsync<PreviewUser>($"User #{user.Id}", parameters);
    }

    public Task HandleAddUser()
    {
        _nav.NavigateTo("/users/create");
        return Task.CompletedTask;
    }

    public Task HandleEditUser(UserResultDto user)
    {
        _nav.NavigateTo($"/users/edit/{user.Id}");
        return Task.CompletedTask;
    }

    public async Task HandleDeleteUser(UserResultDto user)
    {
        bool? confirm = await _dialogService.ShowMessageBox("Confirm Delete", $"Delete user '{user.Username}'?", yesText: "Delete", cancelText: "Cancel", options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall });
        if (confirm != true) return;
        try
        {
            var success = await _usersApi.DeleteUserAsync(user.Id); // Call the API to delete the user
            if (success is true) { await LoadAsync(); _snackbar.Add("User deleted", Severity.Info); }
        }
        catch (Errors.ApiException apiEx) { foreach (var err in apiEx.Errors) _snackbar.Add(err, Severity.Error); }
        catch (Exception ex) { _snackbar.Add($"Unexpected error: {ex.Message}", Severity.Error); }
    }
}
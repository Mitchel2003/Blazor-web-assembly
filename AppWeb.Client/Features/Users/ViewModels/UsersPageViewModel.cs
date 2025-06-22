using AppWeb.Client.Features.Users.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using AppWeb.Client.Services;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;
using MudBlazor;

namespace AppWeb.Client.Features.Users.ViewModels;

public partial class UsersPageViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    private readonly IUsersApiClient _usersApi;
    private readonly ISnackbar _snackbar;

    [ObservableProperty]
    private IReadOnlyList<UserListDto>? users;
    public UsersPageViewModel(IUsersApiClient usersApi, IDialogService dialogService, ISnackbar snackbar)
    {
        _usersApi = usersApi;
        _dialogService = dialogService;
        _snackbar = snackbar;
    }

    public async Task LoadAsync() { Users = await _usersApi.GetUsersAsync(); }

    public async Task HandleAddUser()
    {
        var dialog = await _dialogService.ShowAsync<FormUser>("Create User");
        var result = await dialog.Result;

        if (result != null && !result.Canceled && result.Data is CreateUserInput input)
        {
            var created = await _usersApi.CreateUserAsync(input);
            if (created is not null) { await LoadAsync(); _snackbar.Add("User created successfully", Severity.Success); }
            else { _snackbar.Add("Failed to create user", Severity.Error); }
        }
    }
}
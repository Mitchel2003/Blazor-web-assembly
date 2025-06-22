using Microsoft.AspNetCore.Components;
using AppWeb.Client.Services;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;

namespace AppWeb.Client.Pages;

public partial class Users : ComponentBase
{
    [Inject] private IUsersApiClient UsersApi { get; set; } = default!;
    [Inject] private MudBlazor.IDialogService DialogService { get; set; } = default!;
    [Inject] private MudBlazor.ISnackbar Snackbar { get; set; } = default!;
    protected IReadOnlyList<UserListDto>? users { get; private set; }

    protected override async Task OnInitializedAsync() => users = await UsersApi.GetUsersAsync();

    private async Task HandleAddUser()
    {
        var dialog = await DialogService.ShowAsync<Components.UserForm>("Create User");
        var result = await dialog.Result;
        if (result != null && result.Canceled && result.Data is CreateUserInput input)
        {
            var created = await UsersApi.CreateUserAsync(input);
            if (created is not null) { users = await UsersApi.GetUsersAsync(); Snackbar.Add("User created successfully", MudBlazor.Severity.Success); }
            else { Snackbar.Add("Failed to create user", MudBlazor.Severity.Error); }
            StateHasChanged();
        }
    }
}
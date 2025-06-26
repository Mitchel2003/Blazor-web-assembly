using Microsoft.AspNetCore.Components;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;
using MudBlazor;

namespace AppWeb.Client.Features.Users.Pages;

public partial class UserFormPage : ComponentBase
{
    [Inject] private Services.IUsersApiClient _usersApi { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Parameter] public int? Id { get; set; }
    private bool _isLoading = true;
    private UserResultDto? _existing;

    protected override async Task OnInitializedAsync()
    {
        if (Id is not null) { _existing = await _usersApi.GetUserByIdAsync(Id.Value); }
        _isLoading = false;
    }

    private async Task HandleValidSubmit(object payload)
    {
        try
        {
            if (payload is CreateUserInput create)
            {
                var created = await _usersApi.CreateUserAsync(create);
                if (created is not null) Snackbar.Add("User created successfully", Severity.Success);
            }
            else if (payload is UpdateUserInput update)
            {
                var updated = await _usersApi.UpdateUserAsync(update);
                if (updated is not null) Snackbar.Add("User updated successfully", Severity.Success);
            }
            Nav.NavigateTo("/users");
        }
        catch (Errors.ApiException apiEx) { foreach (var err in apiEx.Errors) Snackbar.Add(err, Severity.Error); }
        catch (Exception ex) { Snackbar.Add($"Unexpected error: {ex.Message}", Severity.Error); }
    }
}
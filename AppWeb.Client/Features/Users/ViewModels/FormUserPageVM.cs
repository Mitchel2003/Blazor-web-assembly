using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using AppWeb.Client.Services;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;
using MudBlazor;

namespace AppWeb.Client.Features.Users.ViewModels;

/// <summary>
/// View-model for FormUserPage. Encapsulates UI state and orchestrates
/// communication with <see cref="IUsersApiClient"/>.
/// </summary>
public partial class FormUserPageVM : ObservableObject
{
    private readonly IUsersApiClient _usersApi;
    private readonly NavigationManager _nav;
    private readonly ISnackbar _snackbar;

    #region Observable ------------------------------------------------------------
    [ObservableProperty] private bool isLoading = true;
    [ObservableProperty] private UserResultDto? existing;
    #endregion ---------------------------------------------------------------------

    public FormUserPageVM(IUsersApiClient usersApi, NavigationManager nav, ISnackbar snackbar)
    {
        _usersApi = usersApi;
        _snackbar = snackbar;
        _nav = nav;
    }

    /// <summary> Loads an existing user (edit scenario) or prepares the page for creation mode. </summary>
    public async Task LoadAsync(int? id, CancellationToken ct = default)
    {
        Existing = null;
        IsLoading = true;
        if (id is not null) { Existing = await _usersApi.GetUserByIdAsync(id.Value, ct); }
        IsLoading = false;
    }

    /// <summary> Handles form submission coming from <see cref="Components.FormUser"/>. </summary>
    public async Task HandleValidSubmit(object payload)
    {
        try
        {
            if (payload is CreateUserInput create)
            {
                var created = await _usersApi.CreateUserAsync(create);
                if (created is not null) _snackbar.Add("User created successfully", Severity.Success);
            }
            else if (payload is UpdateUserInput update)
            {
                var updated = await _usersApi.UpdateUserAsync(update);
                if (updated is not null) _snackbar.Add("User updated successfully", Severity.Success);
            }
            _nav.NavigateTo("/users");
        }
        catch (Errors.ApiException apiEx) { foreach (var err in apiEx.Errors) _snackbar.Add(err, Severity.Error); }
        catch (Exception ex) { _snackbar.Add($"Unexpected error: {ex.Message}", Severity.Error); }
    }
}
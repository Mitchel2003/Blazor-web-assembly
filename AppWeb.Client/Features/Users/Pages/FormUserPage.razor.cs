using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using AppWeb.Client.Services;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;
using MudBlazor;

namespace AppWeb.Client.Features.Users.Pages;

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
    [ObservableProperty] private bool saveSuccess = false;
    [ObservableProperty] private bool isLoading = true;
    [ObservableProperty] private bool isSaving = false;
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
        SaveSuccess = false;
        try
        { if (id is not null) { Existing = await _usersApi.GetUserByIdAsync(id.Value, ct); } }
        catch (Errors.ApiException apiEx) { foreach (var err in apiEx.Errors) _snackbar.Add(err, Severity.Error); }
        catch (Exception ex) { _snackbar.Add($"Error loading user: {ex.Message}", Severity.Error); }
        finally { IsLoading = false; }
    }

    /// <summary> Handles form submission coming from <see cref="Components.FormUser"/>. </summary>
    public async Task HandleValidSubmit(object payload)
    {
        IsSaving = true;
        SaveSuccess = false;
        try
        {
            if (payload is CreateUserInput create)
            {
                var hasCreated = await _usersApi.CreateUserAsync(create);
                if (hasCreated == null) { _snackbar.Add("User create failed", Severity.Error); return; }
                _snackbar.Add("User created successfully", Severity.Success);
                SaveSuccess = true;
                await Task.Delay(1500);
                _nav.NavigateTo("/users");
            }
            else if (payload is UpdateUserInput update)
            {
                var hasUpdated = await _usersApi.UpdateUserAsync(update);
                if (hasUpdated == null) { _snackbar.Add("User update failed", Severity.Error); return; }
                _snackbar.Add("User updated successfully", Severity.Success);
                SaveSuccess = true;
                await Task.Delay(1500);
                _nav.NavigateTo("/users");
            }
        }
        catch (Errors.ApiException apiEx) { foreach (var err in apiEx.Errors) _snackbar.Add(err, Severity.Error); }
        catch (Exception ex) { _snackbar.Add($"Unexpected error: {ex.Message}", Severity.Error); }
        finally { IsSaving = false; }
    }
}
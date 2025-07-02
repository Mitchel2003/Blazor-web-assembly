using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Components;
using CommunityToolkit.Mvvm.Input;
using FluentValidation.Results;
using MudBlazor;

using AppWeb.Shared.Validators;
using AppWeb.Client.Services;
using AppWeb.Shared.Inputs;
using AppWeb.Client.Auth;

namespace AppWeb.Client.Features.Auth.Pages;

/// <summary>View-model that encapsulates login UI state and server interaction.</summary>
public partial class LoginPageVM : ObservableObject
{
    private readonly JwtAuthStateProvider _auth;
    private readonly IAuthApiClient _authApi;
    private readonly NavigationManager _nav;
    private readonly ISnackbar _snackbar;

    [ObservableProperty] private bool _loginSuccess = false;
    [ObservableProperty] private bool _showPassword = false;
    [ObservableProperty] private LoginInput _input = new();
    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _returnUrl = "/";

    private readonly LoginInputValidator _loginValidator = new();

    public LoginPageVM(IAuthApiClient authApi, NavigationManager nav, JwtAuthStateProvider auth, ISnackbar snackbar)
    {
        _authApi = authApi;
        _snackbar = snackbar;
        _auth = auth;
        _nav = nav;
    }

    /// <summary>Checks if the user is authenticated and should be redirected away from login page.</summary>
    public async Task<bool> ShouldRedirectAuthenticatedUserAsync() => await _auth.IsAuthenticatedAsync();

    public async Task InitializeAsync()
    { //Check if the user is already authenticated
        if (await ShouldRedirectAuthenticatedUserAsync()) { _nav.NavigateTo("/"); return; }
        var uri = _nav.ToAbsoluteUri(_nav.Uri); //Get return URL from query string if present
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("returnUrl", out var returnUrl))
        { ReturnUrl = returnUrl!; }
    }

    [RelayCommand]
    public Task TogglePasswordVisibility()
    {
        ShowPassword = !ShowPassword;
        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task Login(MudForm form)
    {
        if (form == null) { _snackbar.Add("Form validation failed", Severity.Error); return; }
        try
        {
            IsLoading = true;
            await form.Validate();
            ValidationResult fvResult = _loginValidator.Validate(Input);
            if (!form.IsValid || !fvResult.IsValid) { IsLoading = false; return; }
            // Continue with login process, calling the API authentication endpoint
            var loginResult = await _authApi.LoginAsync(Input); //Send record LoginInput
            if (loginResult == null) { _snackbar.Add("Login failed", Severity.Error); IsLoading = false; return; }
            // Store the JWT token in localStorage and update authentication state
            await _auth.SetToken(loginResult.Token);
            LoginSuccess = true;
            //Success msg visible
            await Task.Delay(1000);
            _nav.NavigateTo(ReturnUrl);
        }
        catch (Errors.ApiException apiEx) { foreach (var err in apiEx.Errors) _snackbar.Add(err, Severity.Error); }
        catch (Exception ex) { _snackbar.Add($"Error durante el inicio de sesión: {ex.Message}", Severity.Error); }
        finally { IsLoading = false; }
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using AppWeb.Client.Services;
using AppWeb.Shared.Inputs;
using AppWeb.Client.Auth;
using MudBlazor;

namespace AppWeb.Client.Features.Auth.ViewModels;

/// <summary>View-model that encapsulates login UI state and server interaction.</summary>
public partial class LoginPageVM : ObservableObject
{
    private readonly JwtAuthStateProvider _auth;
    private readonly IAuthApiClient _authApi;
    private readonly NavigationManager _nav;
    private readonly ISnackbar _snackbar;

    [ObservableProperty] private LoginInput input = new();

    public LoginPageVM(IAuthApiClient authApi, NavigationManager nav, JwtAuthStateProvider auth, ISnackbar snackbar)
    {
        _authApi = authApi;
        _snackbar = snackbar;
        _auth = auth;
        _nav = nav;
    }

    /// <summary>Checks if the user is authenticated and should be redirected away from login page.</summary>
    public async Task<bool> ShouldRedirectAuthenticatedUserAsync() { return await _auth.IsAuthenticatedAsync(); }

    public async Task<bool> HandleLogin(string returnUrl = "/")
    {
        try
        {
            var hasLogged = await _authApi.LoginAsync(Input!);
            if (hasLogged == null) { _snackbar.Add("Login failed", Severity.Error); return false; }
            //Almacenar el token y navegar (auth)
            await _auth.SetToken(hasLogged.Token);
            _nav.NavigateTo(returnUrl);
            return true;
        }
        catch (Errors.ApiException apiEx) { foreach (var err in apiEx.Errors) _snackbar.Add(err, Severity.Error); return false; }
        catch (Exception ex) { _snackbar.Add($"Error durante el inicio de sesión: {ex.Message}", Severity.Error); return false; }
    }
}
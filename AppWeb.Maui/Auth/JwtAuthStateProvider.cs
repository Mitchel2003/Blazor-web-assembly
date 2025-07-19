using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppWeb.Maui.Auth;

/// <summary>
/// AuthenticationStateProvider for MAUI that uses SecureStorage for token management.
/// Keeps the ClaimsPrincipal in sync. Call <see cref="SetToken"/> after login/refresh.
/// </summary>
public sealed class JwtAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _anon = new(new ClaimsIdentity());
    private AuthenticationState? _currentState;
    private const string StorageKey = "jwt";

    /// <summary>
    /// Event that is triggered when the authentication state changes.
    /// The boolean parameter indicates whether the user is authenticated.
    /// </summary>
    public new event EventHandler<bool>? AuthenticationStateChanged;

    public JwtAuthStateProvider() { }

    /// <summary>Gets the JWT token from SecureStorage.</summary>
    public async Task<string?> GetTokenAsync()
    { return await SecureStorage.GetAsync(StorageKey); }

    /// <summary>Checks if the user is currently authenticated.</summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }

    /// <summary>Parses the JWT and returns its claims.</summary>
    private static IEnumerable<Claim> ParseClaims(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }

    /// <summary>Gets the username from the JWT claims.</summary>
    public string? GetUsername()
    {
        if (_currentState?.User?.Identity?.IsAuthenticated != true) return null;
        //Try to get the username from various possible claim types
        return _currentState.User.FindFirst(ClaimTypes.Name)?.Value
            ?? _currentState.User.FindFirst("name")?.Value
            ?? _currentState.User.FindFirst("email")?.Value;
    }

    /// <summary>Gets the JWT token from SecureStorage and builds an authentication state.</summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_currentState != null) { return _currentState; }
        var token = await GetTokenAsync(); //Get the token from SecureStorage
        if (string.IsNullOrWhiteSpace(token)) { _currentState = new AuthenticationState(_anon); return _currentState; }
        var identity = new ClaimsIdentity(ParseClaims(token), "jwt");
        _currentState = new AuthenticationState(new(identity));
        return _currentState;
    }

    /// <summary>Sets the JWT token in SecureStorage and updates the authentication state.</summary>
    public async Task SetToken(string? token)
    { //Always reset current state to ensure fresh state evaluation
        _currentState = null;
        bool isAuthenticated;

        if (string.IsNullOrWhiteSpace(token))
        { //Clear token from storage
            SecureStorage.Remove(StorageKey);

            _currentState = new AuthenticationState(_anon);
            NotifyAuthenticationStateChanged(Task.FromResult(_currentState));
            isAuthenticated = false;
        }
        else
        { //Store token in secure storage
            await SecureStorage.SetAsync(StorageKey, token);

            var identity = new ClaimsIdentity(ParseClaims(token), "jwt");
            _currentState = new AuthenticationState(new(identity));
            NotifyAuthenticationStateChanged(Task.FromResult(_currentState));
            isAuthenticated = true;
        }

        //Trigger the authentication state changed event
        AuthenticationStateChanged?.Invoke(this, isAuthenticated);
        await Task.Delay(100); //Small delay to ensure changes propagate
    }
}
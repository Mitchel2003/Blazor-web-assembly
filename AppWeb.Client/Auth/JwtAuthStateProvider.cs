using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.JSInterop;

namespace AppWeb.Client.Auth;

/// <summary>
/// AuthenticationStateProvider that reads a JWT from browser localStorage (key: "jwt")
/// and keeps the ClaimsPrincipal in sync. Call <see cref="SetToken"/> after login/refresh.
/// </summary>
public sealed class JwtAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _anon = new(new ClaimsIdentity());
    private AuthenticationState? _currentState;
    private const string StorageKey = "jwt";
    private readonly IJSRuntime _js;

    public JwtAuthStateProvider(IJSRuntime js) { _js = js; }

    /// <summary> Gets the JWT token from localStorage. </summary>
    public async Task<string?> GetTokenAsync() => await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);

    /// <summary> Checks if the user is currently authenticated. </summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }

    /// <summary> Parses the JWT and returns its claims. </summary>
    private static IEnumerable<Claim> ParseClaims(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }

    /// <summary> Gets the JWT token from localStorage. </summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_currentState != null) { return _currentState; }
        var token = await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);
        if (string.IsNullOrWhiteSpace(token)) { _currentState = new AuthenticationState(_anon); return _currentState; }
        var identity = new ClaimsIdentity(ParseClaims(token), "jwt");
        _currentState = new AuthenticationState(new(identity));
        return _currentState;
    }

    /// <summary> Sets the JWT token in localStorage and updates the authentication state. </summary>
    public async Task SetToken(string? token)
    { //always reset current state to ensure fresh state evaluation
        _currentState = null;    
        if (string.IsNullOrWhiteSpace(token))
        { //clear token from storage
            await _js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
            //force browser to refresh auth cookies, clear cookie
            await _js.InvokeVoidAsync("eval", "document.cookie = 'jwt=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;'");            
            _currentState = new AuthenticationState(_anon); //set auth state
            NotifyAuthenticationStateChanged(Task.FromResult(_currentState));
            await Task.Delay(100); //force a small delay to ensure state propagation
        }
        else
        {
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, token);
            var identity = new ClaimsIdentity(ParseClaims(token), "jwt");
            _currentState = new AuthenticationState(new(identity));
            NotifyAuthenticationStateChanged(Task.FromResult(_currentState));
            await Task.Delay(100); //force a small delay to ensure state propagation
        }
    }
}
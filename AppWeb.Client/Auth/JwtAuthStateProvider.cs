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
    private const string StorageKey = "jwt";
    private readonly IJSRuntime _js;

    public JwtAuthStateProvider(IJSRuntime js) => _js = js;

    /// <summary> Gets the JWT token from localStorage. </summary>
    public async Task<string?> GetTokenAsync() => await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);

    /// <summary> Gets the JWT token from localStorage. </summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);
        if (string.IsNullOrWhiteSpace(token)) return new AuthenticationState(_anon);
        var identity = new ClaimsIdentity(ParseClaims(token), "jwt");
        return new AuthenticationState(new(identity));
    }

    /// <summary> Sets the JWT token in localStorage and updates the authentication state. </summary>
    public async Task SetToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anon)));
        }
        else
        {
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, token);
            var identity = new ClaimsIdentity(ParseClaims(token), "jwt");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new(identity))));
        }
    }

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
}
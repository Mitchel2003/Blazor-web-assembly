using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;

namespace AppWeb.Shared.Services.Contracts;

/// <summary>Abstraction of the user service for the ViewModels</summary>
public interface IAuthService
{
    /// <summary>Event raised when the authentication state changes</summary>
    event EventHandler<AuthenticationStateChangedEventArgs>? AuthenticationStateChanged;
    
    /// <summary>Sets the JWT token for authentication.</summary>
    Task SetTokenAsync(string? token);
    
    /// <summary>Checks if user is currently authenticated.</summary>
    Task<bool> IsAuthenticatedAsync();
    
    /// <summary>Authenticates user with given credentials.</summary>
    Task<LoginResultDto?> LoginAsync(LoginInput input, CancellationToken ct = default);
    
    /// <summary>Logs out the current user.</summary>
    Task LogoutAsync(CancellationToken ct = default);
}

/// <summary>Event args for authentication state changes</summary>
public class AuthenticationStateChangedEventArgs : EventArgs
{
    /// <summary>Indicates whether the user is authenticated</summary>
    public bool IsAuthenticated { get; set; }
    
    /// <summary>The username of the authenticated user, if available</summary>
    public string? Username { get; set; }
} 
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;

namespace AppWeb.ViewModels.Core.Services;

/// <summary>Abstracción del servicio de usuarios para los ViewModels</summary>
public interface IAuthService
{
    /// <summary>Sets the JWT token for authentication.</summary>
    Task SetTokenAsync(string? token);
    
    /// <summary>Checks if user is currently authenticated.</summary>
    Task<bool> IsAuthenticatedAsync();
    
    /// <summary>Authenticates user with given credentials.</summary>
    Task<LoginResultDto?> LoginAsync(LoginInput input, CancellationToken ct = default);
    
    /// <summary>Logs out the current user.</summary>
    Task LogoutAsync(CancellationToken ct = default);
}
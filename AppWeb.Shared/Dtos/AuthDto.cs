namespace AppWeb.Shared.Dtos;

public record LoginDto(string Email, string Password);

/// <summary>DTO returned after a successful login.</summary>
public record LoginResultDto(int UserId, string Email, string Token)
{/// <summary>Username of the logged-in user. Defaults to Email if not explicitly set.</summary>
    public string Username => Email;
}

public class AuthResultDto
{
    /// <summary>Indicates whether the authentication was successful.</summary>
    public bool Success { get; set; }
    /// <summary>Optional error message if authentication failed.</summary>
    public string? Error { get; set; }
    /// <summary>Message providing additional information about the authentication result.</summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>Authentication token returned upon successful login.</summary>
    public string? Token { get; set; }
    /// <summary>Data returned upon successful authentication.</summary>
    public UserResultDto? User { get; set; }
}
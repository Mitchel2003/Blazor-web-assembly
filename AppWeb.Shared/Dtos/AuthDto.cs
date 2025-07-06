namespace AppWeb.Shared.Dtos;

public record LoginDto(string Email, string Password);
/// <summary>DTO returned after a successful login.</summary>
public record LoginResultDto(int UserId, string Email, string Token)
{
    /// <summary>Username of the logged-in user. Defaults to Email if not explicitly set.</summary>
    public string Username => Email;
}
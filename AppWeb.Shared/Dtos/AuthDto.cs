namespace AppWeb.Shared.Dtos;

public record LoginDto(string Email, string Password);
/// <summary>DTO returned after a successful login.</summary>
public record LoginResultDto(int UserId, string Email, string Token);
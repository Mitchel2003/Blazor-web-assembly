namespace AppWeb.Shared.Dtos;

public record UserDto(string Username, string Email, string Password, bool IsActive = true);

/// <summary>DTO returned after a successful login.</summary>
public record UserResultDto(int Id, string Username, string Email, string Password, bool IsActive = true);
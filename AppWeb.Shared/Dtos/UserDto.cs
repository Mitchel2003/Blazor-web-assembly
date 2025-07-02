namespace AppWeb.Shared.Dtos;

public record UserDto(string Username, string Email, string Password, bool IsActive = true);
public record UserResultDto(int Id, string Username, string Email, string Password, bool IsActive = true);
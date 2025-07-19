using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;

namespace AppWeb.Shared.Services.Contracts;

/// <summary>Abstraction of the user service for the ViewModels</summary>
public interface IUsersService
{
    Task<UserResultDto?> GetUserByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<UserResultDto>> GetUsersAsync(CancellationToken ct = default);
    Task<UserResultDto?> CreateUserAsync(CreateUserInput input, CancellationToken ct = default);
    Task<UserResultDto?> UpdateUserAsync(UpdateUserInput input, CancellationToken ct = default);
    Task<bool> DeleteUserAsync(int id, CancellationToken ct = default);
}
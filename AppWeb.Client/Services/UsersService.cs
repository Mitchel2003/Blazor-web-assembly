using AppWeb.ViewModels.Core.Services;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;

namespace AppWeb.Client.Services;

public class BlazorUsersService : IUsersService
{
    private readonly IUsersApiClient _apiClient;

    public BlazorUsersService(IUsersApiClient apiClient)
    { _apiClient = apiClient; }

    public Task<UserResultDto?> GetUserByIdAsync(int id, CancellationToken ct = default)
        => _apiClient.GetUserByIdAsync(id, ct);

    public Task<IReadOnlyList<UserResultDto>> GetUsersAsync(CancellationToken ct = default)
        => _apiClient.GetUsersAsync(ct);

    public Task<UserResultDto?> CreateUserAsync(CreateUserInput input, CancellationToken ct = default)
        => _apiClient.CreateUserAsync(input, ct);

    public Task<UserResultDto?> UpdateUserAsync(UpdateUserInput input, CancellationToken ct = default)
        => _apiClient.UpdateUserAsync(input, ct);

    public async Task<bool> DeleteUserAsync(int id, CancellationToken ct = default)
    {
        var result = await _apiClient.DeleteUserAsync(id, ct);
        return result == true;
    }
}
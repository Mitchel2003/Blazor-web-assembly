using System.Net.Http.Json;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;

namespace AppWeb.Client.Services;

/// <summary> GraphQL-based client for retrieving Users data. </summary>
public class UsersApiClient : IUsersApiClient
{
    private readonly HttpClient _http;
    private const string QueryUsers = "query { users { id username email } }";
    private const string MutationCreateUser = "mutation ($input: CreateUserInput!) { createUser(dto: $input) { id username email } }";
    public UsersApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<UserListDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var requestBody = new { query = QueryUsers };
        var response = await _http.PostAsJsonAsync("/graphql", requestBody, cancellationToken);
        response.EnsureSuccessStatusCode();

        var gql = await response.Content.ReadFromJsonAsync<GqlResponse>(cancellationToken: cancellationToken);
        return gql?.Data?.Users ?? new List<UserListDto>();
    }

    public async Task<UserListDto?> CreateUserAsync(CreateUserInput input, CancellationToken cancellationToken = default)
    {
        var requestBody = new
        {
            query = MutationCreateUser,
            variables = new { input }
        };
        var response = await _http.PostAsJsonAsync("/graphql", requestBody, cancellationToken);
        response.EnsureSuccessStatusCode();
        var gql = await response.Content.ReadFromJsonAsync<CreateUserGqlResponse>(cancellationToken: cancellationToken);
        return gql?.Data?.CreateUser;
    }
    // <summary> Response classes for deserializing GraphQL responses to getUsers. </summary>
    private sealed class GqlResponse { public DataWrapper? Data { get; set; } }
    private sealed class DataWrapper { public List<UserListDto>? Users { get; set; } }
    // <summary> Response classes for deserializing GraphQL responses to crateUser. </summary>
    private sealed class CreateUserGqlResponse { public CreateWrapper? Data { get; set; } }
    private sealed class CreateWrapper { public UserListDto? CreateUser { get; set; } }
}

#region Interface ------------------------------------------------------------
public interface IUsersApiClient
{
    Task<IReadOnlyList<UserListDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<UserListDto?> CreateUserAsync(CreateUserInput input, CancellationToken cancellationToken = default);
}
#endregion ---------------------------------------------------------------------
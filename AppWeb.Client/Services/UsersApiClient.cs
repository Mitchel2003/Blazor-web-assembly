using System.Net.Http.Json;
using AppWeb.Shared.Inputs;
using AppWeb.Client.Http;
using AppWeb.Shared.Dtos;
using System.Text.Json;

namespace AppWeb.Client.Services;

/// <summary> GraphQL-based client for retrieving Users data. </summary>
public class UsersApiClient : IUsersApiClient
{
    private static readonly JsonSerializerOptions Camel = new(JsonSerializerDefaults.Web);
    private const string MutationCreateUser = "mutation ($dto: UserDtoInput!) { createUser(dto: $dto) { id username email } }";
    private const string QueryUsers = "query { users { id username email } }";
    private readonly HttpClient _http;
    
    public UsersApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<UserListDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var requestBody = new { query = QueryUsers };
        //After build request body, we process it; we use PostAsJsonAsync to send it to the server
        var response = await _http.PostAsJsonAsync("/graphql", requestBody, Camel, cancellationToken);
        var json = await response.EnsureGraphQLSuccessAsync(cancellationToken); //Ensure success
        var gql = JsonSerializer.Deserialize<GqlResponse>(json, Camel);
        return gql?.Data?.Users ?? new List<UserListDto>();
    }

    public async Task<UserListDto?> CreateUserAsync(CreateUserInput input, CancellationToken cancellationToken = default)
    {
        var dto = new UserDto(input.Username, input.Email, input.Password);
        var requestBody = new { query = MutationCreateUser, variables = new { dto } };
        //After build request body, we process it; we use PostAsJsonAsync to send it to the server
        var response = await _http.PostAsJsonAsync("/graphql", requestBody, Camel, cancellationToken);
        var payload = await response.EnsureGraphQLSuccessAsync(cancellationToken); //Ensure success
        var gql = JsonSerializer.Deserialize<CreateUserGqlResponse>(payload, Camel);
        return gql?.Data?.CreateUser;
    }

    // <summary> Response classes for deserializing GraphQL responses to getUsers. </summary>
    private sealed class GqlResponse { public DataWrapper? Data { get; set; } }
    private sealed class DataWrapper { public List<UserListDto>? Users { get; set; } }
    // <summary> Response classes for deserializing GraphQL responses to createUser. </summary>
    private sealed class CreateUserGqlResponse { public CreateWrapper? Data { get; set; } }
    private sealed class CreateWrapper { public UserListDto? CreateUser { get; set; } }
}

#region Interfaces ------------------------------------------------------------
public interface IUsersApiClient
{
    Task<IReadOnlyList<UserListDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<UserListDto?> CreateUserAsync(CreateUserInput input, CancellationToken cancellationToken = default);
}
#endregion ---------------------------------------------------------------------
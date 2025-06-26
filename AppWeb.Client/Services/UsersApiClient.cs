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

    #region Operations ------------------------------------------------------------
    private const string MutationCreateUser = "mutation ($dto: UserDtoInput!) { createUser(dto: $dto) { id username email } }";
    private const string MutationUpdateUser = "mutation ($id: Int!, $dto: UserDtoInput!) { updateUser(id: $id, dto: $dto) { id username email } }";
    private const string MutationDeleteUser = "mutation ($id: Int!) { deleteUser(id: $id) }";

    private const string QueryUsers = "query { users { id username email } }";
    private const string QueryUserById = "query ($id: Int!) { userById(id: $id) { id username email password } }";
    #endregion ---------------------------------------------------------------------

    private readonly HttpClient _http;

    public UsersApiClient(HttpClient http) => _http = http;

    #region Queries ------------------------------------------------------------
    public async Task<IReadOnlyList<UserResultDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var requestBody = new { query = QueryUsers };
        //After build request body, we process it; we use PostAsJsonAsync to send it to the server
        var response = await _http.PostAsJsonAsync("/graphql", requestBody, Camel, cancellationToken);
        var json = await response.EnsureGraphQLSuccessAsync(cancellationToken); //Ensure success
        var gql = JsonSerializer.Deserialize<GetUsersResponse>(json, Camel);
        return gql?.Data?.Users ?? new List<UserResultDto>();
    }
    public async Task<UserResultDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var requestBody = new { query = QueryUserById, variables = new { id } };
        var response = await _http.PostAsJsonAsync("/graphql", requestBody, Camel, cancellationToken);
        var payload = await response.EnsureGraphQLSuccessAsync(cancellationToken); //Ensure success
        var gql = JsonSerializer.Deserialize<GetUserByIdResponse>(payload, Camel);
        Console.WriteLine(gql?.Data?.UserById);
        return gql?.Data?.UserById;
    }
    #endregion ---------------------------------------------------------------------

    #region Commands ------------------------------------------------------------
    public async Task<UserResultDto?> CreateUserAsync(CreateUserInput input, CancellationToken cancellationToken = default)
    {
        var dto = new { input.Username, input.Email, input.Password }; //Prepare Dto
        var requestBody = new { query = MutationCreateUser, variables = new { dto } }; 
        //After build request body, we process it; we use PostAsJsonAsync to send it to the server
        var response = await _http.PostAsJsonAsync("/graphql", requestBody, Camel, cancellationToken);
        var payload = await response.EnsureGraphQLSuccessAsync(cancellationToken); //Ensure success
        var gql = JsonSerializer.Deserialize<CreateUserResponse>(payload, Camel);
        return gql?.Data?.CreateUser;
    }
    public async Task<UserResultDto?> UpdateUserAsync(UpdateUserInput input, CancellationToken cancellationToken = default)
    {
        var dto = new { input.Username, input.Email, input.Password }; //Prepare Dto
        var requestBody = new { query = MutationUpdateUser, variables = new { id = input.Id, dto } };
        var response = await _http.PostAsJsonAsync("/graphql", requestBody, Camel, cancellationToken);
        var payload = await response.EnsureGraphQLSuccessAsync(cancellationToken);
        var gql = JsonSerializer.Deserialize<UpdateUserResponse>(payload, Camel);
        return gql?.Data?.UpdateUser;
    }
    public async Task<bool?> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var requestBody = new { query = MutationDeleteUser, variables = new { id } };
        //After build request body, we process it; we use PostAsJsonAsync to send it to the server
        var response = await _http.PostAsJsonAsync("/graphql", requestBody, Camel, cancellationToken);
        var payload = await response.EnsureGraphQLSuccessAsync(cancellationToken); //Ensure success
        var gql = JsonSerializer.Deserialize<DeleteUserResponse>(payload, Camel);
        return gql?.Data?.DeleteUser;
    }
    #endregion ---------------------------------------------------------------------

    #region Results ------------------------------------------------------------
    //Response classes for deserializing GraphQL responses to getUsers.
    private sealed class GetUsersResponse { public DataWrapper? Data { get; set; } }
    private sealed class DataWrapper { public List<UserResultDto>? Users { get; set; } }
    //Response classes for deserializing GraphQL responses to getUserById.
    private sealed class GetUserByIdResponse { public GetWrapper? Data { get; set; } }
    private sealed class GetWrapper { public UserResultDto? UserById { get; set; } }
    //Response classes for deserializing GraphQL responses to createUser.
    private sealed class CreateUserResponse { public CreateWrapper? Data { get; set; } }
    private sealed class CreateWrapper { public UserResultDto? CreateUser { get; set; } }
    //Response classes for deserializing GraphQL responses to updateUser.
    private sealed class UpdateUserResponse { public UpdateWrapper? Data { get; set; } }
    private sealed class UpdateWrapper { public UserResultDto? UpdateUser { get; set; } }
    //Response classes for deserializing GraphQL responses to deleteUser.
    private sealed class DeleteUserResponse { public DeleteWrapper? Data { get; set; } }
    private sealed class DeleteWrapper { public bool? DeleteUser { get; set; } }
    #endregion ---------------------------------------------------------------------
}

#region Interfaces ------------------------------------------------------------
public interface IUsersApiClient
{
    Task<IReadOnlyList<UserResultDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<UserResultDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserResultDto?> CreateUserAsync(CreateUserInput input, CancellationToken cancellationToken = default);
    Task<UserResultDto?> UpdateUserAsync(UpdateUserInput input, CancellationToken cancellationToken = default);
    Task<bool?> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
}
#endregion ---------------------------------------------------------------------
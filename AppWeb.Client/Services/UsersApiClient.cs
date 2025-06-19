using System.Net.Http.Json;
using AppWeb.Shared.Dtos;

namespace AppWeb.Client.Services;

/// <summary> GraphQL-based client for retrieving Users data. </summary>
public class UsersApiClient : IUsersApiClient
{
    private readonly HttpClient _http;
    private const string GraphQlQuery = "query { users { id username email } }";
    public UsersApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<UserListDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {   
        var requestBody = new { query = GraphQlQuery };
        var response = await _http.PostAsJsonAsync("/graphql", requestBody, cancellationToken);
        response.EnsureSuccessStatusCode();

        var gql = await response.Content.ReadFromJsonAsync<GqlResponse>(cancellationToken: cancellationToken);
        return gql?.Data?.Users ?? new List<UserListDto>();
    }

    private sealed class GqlResponse { public DataWrapper? Data { get; set; } }
    private sealed class DataWrapper { public List<UserListDto>? Users { get; set; } }
}
/*---------------------------------------------------------------------------------------------------------*/

/*--------------------------------------------------Interface--------------------------------------------------*/
public interface IUsersApiClient
{ Task<IReadOnlyList<UserListDto>> GetUsersAsync(CancellationToken cancellationToken = default); }
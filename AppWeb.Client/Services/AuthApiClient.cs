using System.Net.Http.Json;
using AppWeb.Client.Errors;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;
using System.Text.Json;

namespace AppWeb.Client.Services;

/// <summary> API client for authentication operations. </summary>
public class AuthApiClient : IAuthApiClient
{
    private static readonly JsonSerializerOptions Camel = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _http;

    public AuthApiClient(HttpClient http) => _http = http;

    #region Commands ------------------------------------------------------------
    public async Task<LoginResultDto?> LoginAsync(LoginInput input, CancellationToken cancellationToken = default)
    {
        var loginDto = new LoginDto(input.Email, input.Password);
        var response = await _http.PostAsJsonAsync("api/auth/login", loginDto, cancellationToken);
        //verify status code and process errors
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            try
            {
                var errors = await response.Content.ReadFromJsonAsync<string[]>(cancellationToken) ?? new[] { "Error de autenticación" };
                throw new ApiException(response.StatusCode, errors);
            }
            catch (JsonException){ throw new ApiException(response.StatusCode, new[] { errorContent }); }
        }
        
        //deserialize successful response
        return await response.Content.ReadFromJsonAsync<LoginResultDto>(cancellationToken);
    }
    
    /// <summary>Logs out the current user by calling the server logout endpoint.</summary>
    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        try { await _http.PostAsync("api/auth/logout", null, cancellationToken); }
        catch (Exception) { /* Ignore errors, as we'll clear the local token anyway */ }
    }
    #endregion ---------------------------------------------------------------------

    #region Results ------------------------------------------------------------
    //Response classes for deserializing GraphQL responses to getUsers.
    private sealed class GetAuthResponse { public DataWrapper? Data { get; set; } }
    private sealed class DataWrapper { public LoginResultDto? User { get; set; } }
    #endregion ---------------------------------------------------------------------
}

#region Interfaces ------------------------------------------------------------
public interface IAuthApiClient
{
    /// <summary>Authenticates user with given credentials.</summary>
    Task<LoginResultDto?> LoginAsync(LoginInput input, CancellationToken cancellationToken = default);
    
    /// <summary>Logs out the current user.</summary>
    Task LogoutAsync(CancellationToken cancellationToken = default);
}
#endregion ---------------------------------------------------------------------
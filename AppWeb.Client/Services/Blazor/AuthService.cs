using AppWeb.ViewModels.Core.Services;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;
using AppWeb.Client.Auth;

namespace AppWeb.Client.Services.Blazor
{
    /// <summary>
    /// Implementation of IAuthService for Blazor client, using AuthApiClient for API communication
    /// and JwtAuthStateProvider for token management.
    /// </summary>
    public class BlazorAuthService : IAuthService
    {
        private readonly IAuthApiClient _authApiClient;
        private readonly JwtAuthStateProvider _authStateProvider;

        public BlazorAuthService(IAuthApiClient authApiClient, JwtAuthStateProvider authStateProvider)
        { _authApiClient = authApiClient; _authStateProvider = authStateProvider; }

        /// <summary>Checks if user is currently authenticated by verifying JWT token existence.</summary>
        public Task<bool> IsAuthenticatedAsync() => _authStateProvider.IsAuthenticatedAsync();

        /// <summary>Sets the JWT token for authentication.</summary>
        public async Task SetTokenAsync(string? token) 
        { 
            await _authStateProvider.SetToken(token);
            await Task.Delay(500); //ensure token is fully propagated
        }

        /// <summary>Authenticates user with given credentials via API and sets JWT token on success.</summary>
        public async Task<LoginResultDto?> LoginAsync(LoginInput input, CancellationToken ct = default)
        {
            var result = await _authApiClient.LoginAsync(input, ct);
            //set token and ensure auth state is fully propagated
            if (result != null) 
            { //to ensure clean state
                await SetTokenAsync(null);
                await SetTokenAsync(result.Token); 
            }
            return result;
        }
        
        /// <summary>Logs out the current user by clearing the JWT token.</summary>
        public async Task LogoutAsync(CancellationToken ct = default)
        {
            try { await _authApiClient.LogoutAsync(ct); }
            catch (Exception ex) { Console.WriteLine($"Error logging out from server: {ex.Message}"); }
            finally { await SetTokenAsync(null); await Task.Delay(500); } //force a delay to ensure token clearing is complete
        }
    }
}
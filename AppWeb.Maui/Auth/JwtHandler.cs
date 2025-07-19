using System.Net.Http.Headers;

namespace AppWeb.Maui.Auth;

/// <summary>DelegatingHandler that injects the current JWT into outgoing requests for MAUI.</summary>
public sealed class JwtHandler : DelegatingHandler
{
    private readonly JwtAuthStateProvider _provider;
    public JwtHandler(JwtAuthStateProvider provider)
    { _provider = provider; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _provider.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
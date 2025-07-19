using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace AppWeb.Client.Auth;

/// <summary>DelegatingHandler that injects the current JWT into outgoing requests.</summary>
public sealed class JwtHandler : DelegatingHandler
{
    private readonly JwtAuthStateProvider _provider;
    public JwtHandler(AuthenticationStateProvider provider)
    { _provider = (JwtAuthStateProvider)provider; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _provider.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
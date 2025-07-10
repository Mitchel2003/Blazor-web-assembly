using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using AppWeb.Application.Security;
using System.Security.Claims;
using System.Text;

namespace AppWeb.Infrastructure.Security;

/// <summary>Default HMAC-SHA256 implementation of <see cref="ITokenGenerator"/>.</summary>
public sealed class JwtGenerator : IJwtGenerator
{
    private readonly IConfiguration _cfg;
    private readonly byte[] _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly TimeSpan _lifetime;

    public JwtGenerator(IConfiguration cfg)
    {
        _cfg = cfg;
        _issuer = cfg["Jwt:Issuer"] ?? "AppWeb.Server";
        _audience = cfg["Jwt:Audience"] ?? "AppWeb.Client";
        _lifetime = TimeSpan.TryParse(cfg["Jwt:Lifetime"], out var t) ? t : TimeSpan.FromMinutes(15);
        _key = Encoding.UTF8.GetBytes(cfg["Jwt:Secret"] ?? cfg["Jwt:Key"] ?? throw new InvalidOperationException("JWT secret key is missing"));
    }

    public string Generate(int userId, string email)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email)
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer: _issuer, audience: _audience, claims: claims,
            expires: DateTime.UtcNow.Add(_lifetime),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
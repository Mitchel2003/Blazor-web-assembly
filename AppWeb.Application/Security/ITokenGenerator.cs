namespace AppWeb.Application.Security;

/// <summary>Generates short-lived JSON Web Tokens for API / GraphQL authorization.</summary>
public interface IJwtGenerator
{
    /// <summary>Creates a signed JWT for the specified user.</summary>
    /// <param name="userId">Unique user identifier.</param>
    /// <param name="email">User email (added as claim).</param>
    /// <returns>Encoded JWT string.</returns>
    string Generate(int userId, string email);
}
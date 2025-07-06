using Microsoft.AspNetCore.Identity;
using AppWeb.Application.Security;
using AppWeb.Domain.Interfaces;
using AppWeb.Shared.Dtos;
using MediatR;

namespace AppWeb.Application.Graphql.Handlers;

#region Commands ------------------------------------------------------------
/** Implements the IRequestHandler interface for the Login request. */
public sealed class LoginHandler : IRequestHandler<LoginCommand, LoginResultDto>
{
    private readonly IPasswordHasher<string> _hasher;
    private readonly IUserRepository _repo;
    private readonly IJwtGenerator _jwt;

    public LoginHandler(IUserRepository repo, IPasswordHasher<string> hasher, IJwtGenerator jwt)
    { _repo = repo; _hasher = hasher; _jwt = jwt; } // Constructor authentication handler

    public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken ct)
    {
        PasswordVerificationResult pwd;
        var user = (await _repo.GetByFilterAsync(u => u.Email == request.Input.Email)).FirstOrDefault()
            ?? throw new UnauthorizedAccessException("Invalid credentials"); //User not found or mismatch
        try { pwd = _hasher.VerifyHashedPassword(user.Email, user.Password, request.Input.Password); }
        catch (FormatException)
        { //Stored password is likely in plaintext (legacy data). Fallback to direct comparison.
            if (user.Password != request.Input.Password) throw new UnauthorizedAccessException("Invalid credentials");
            pwd = PasswordVerificationResult.Success; //Optionally, re-hash the password here for future logins (left out for brevity).
        }
        if (pwd == PasswordVerificationResult.Failed) throw new UnauthorizedAccessException("Invalid credentials");
        var token = _jwt.Generate(user.Id, user.Email); //Generate JWT token for the user
        return new LoginResultDto(user.Id, user.Email, token);
    }
}
#endregion ---------------------------------------------------------------------

/** User-specific CQRS records that wrap the generic primitives. */
#region Records ------------------------------------------------------------
public sealed record LoginCommand(LoginDto Input) : IRequest<LoginResultDto>;
#endregion ---------------------------------------------------------------------
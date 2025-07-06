using Microsoft.AspNetCore.Identity;
using AppWeb.Domain.Interfaces;
using AppWeb.Application.Core;
using AppWeb.Domain.Models;
using AppWeb.Shared.Dtos;
using MediatR;

namespace AppWeb.Application.Graphql.Handlers;

#region Queries ------------------------------------------------------------
/** Implements the IRequestHandler interface for the GetUsers request. */
public sealed class GetUsersHandler : IRequestHandler<GetUsers, IEnumerable<User>>
{
    private readonly IUserRepository _repo;
    public GetUsersHandler(IUserRepository repo) => _repo = repo;
    public Task<IEnumerable<User>> Handle(GetUsers request, CancellationToken ct) => _repo.GetAllAsync();
}

/** Implements the IRequestHandler interface for the GetUserById request. */
public sealed class GetUserByIdHandler : IRequestHandler<GetUserById, User?>
{
    private readonly IUserRepository _repo;
    public GetUserByIdHandler(IUserRepository repo) => _repo = repo;
    public Task<User?> Handle(GetUserById request, CancellationToken ct) => _repo.GetByIdAsync(request.Id);
}

/** Implements the IRequestHandler interface for the GetUsersByEmail request. */
public sealed class GetUsersByEmailHandler : IRequestHandler<GetUsersByEmail, IEnumerable<User>>
{
    private readonly IUserRepository _repo;
    public GetUsersByEmailHandler(IUserRepository repo) => _repo = repo;
    public Task<IEnumerable<User>> Handle(GetUsersByEmail request, CancellationToken ct) => _repo.GetByFilterAsync(u => u.Email == request.Email);
}
#endregion ---------------------------------------------------------------------

#region Commands ------------------------------------------------------------
/** Implements the IRequestHandler interface for the CreateUser request. */
public sealed class CreateUserHandler : IRequestHandler<CreateUser, User>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher<string> _hasher;
    public CreateUserHandler(IUserRepository repo, IPasswordHasher<string> hasher)
    { _repo = repo; _hasher = hasher; }

    public async Task<User> Handle(CreateUser request, CancellationToken cancellationToken)
    { //Hash the incoming plaintext password before persisting the new user, auth by hash.
        var hashedPwd = _hasher.HashPassword(request.Input.Email, request.Input.Password);
        var dto = request.Input with { Password = hashedPwd };
        return await _repo.CreateAsync(dto);
    }
}

/** Implements the IRequestHandler interface for the UpdateUser request. */
public sealed class UpdateUserHandler : IRequestHandler<UpdateUser, User>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher<string> _hasher;
    public UpdateUserHandler(IUserRepository repo, IPasswordHasher<string> hasher)
    { _repo = repo; _hasher = hasher; }

    public async Task<User> Handle(UpdateUser request, CancellationToken cancellationToken)
    { //If client provided a plaintext password, hash it; otherwise keep the existing.
        string pass = request.Input.Password; //heuristic: Identity hashes are >60 chars
        if (!string.IsNullOrWhiteSpace(pass)) pass = _hasher.HashPassword(request.Input.Email, pass);
        var dto = request.Input with { Password = pass };
        return await _repo.UpdateAsync(request.Id, dto);
    }
}

/** Implements the IRequestHandler interface for the DeleteUser request. */
public sealed class DeleteUserHandler : IRequestHandler<DeleteUser, bool>
{
    private readonly IUserRepository _repo;
    public DeleteUserHandler(IUserRepository repo) => _repo = repo;
    public Task<bool> Handle(DeleteUser request, CancellationToken cancellationToken) => _repo.DeleteAsync(request.Id);
}
#endregion ---------------------------------------------------------------------

/** User-specific CQRS records that wrap the generic primitives. */
#region Records ------------------------------------------------------------
public sealed record GetUsers() : GetAllQuery<User>();
public sealed record GetUserById(int Id) : GetByIdQuery<User>(Id);
public sealed record GetUsersByEmail(string Email) : GetByFilterQuery<User>(p => p.Email == Email);

public sealed record CreateUser(UserDto dto) : CreateCommand<UserDto, User>(dto);
public sealed record UpdateUser(int Id, UserDto dto) : UpdateCommand<UserDto, User>(Id, dto);
public sealed record DeleteUser(int Id) : DeleteCommand<User>(Id);
#endregion ---------------------------------------------------------------------
using AppWeb.Shared.Dtos;
using AppWeb.Models;
using AppWeb.Core;
using MediatR;

namespace AppWeb.Application.Handlers;

/*--------------------------------------------------queries--------------------------------------------------*/
/** Implements the IRequestHandler interface for the GetUsers request. */
public sealed class GetUsersHandler : IRequestHandler<GetUsers, IEnumerable<User>>
{
    private readonly Query<User> _query;
    public GetUsersHandler(Query<User> query) => _query = query;
    public Task<IEnumerable<User>> Handle(GetUsers request, CancellationToken ct) => _query.GetAll();
}

/** Implements the IRequestHandler interface for the GetUserById request. */
public sealed class GetUserByIdHandler : IRequestHandler<GetUserById, User?>
{
    private readonly Query<User> _query;
    public GetUserByIdHandler(Query<User> query) => _query = query;
    public Task<User?> Handle(GetUserById request, CancellationToken ct) => _query.GetById(request.Id);
}

/** Implements the IRequestHandler interface for the GetUsersByEmail request. */
public sealed class GetUsersByEmailHandler : IRequestHandler<GetUsersByEmail, IEnumerable<User>>
{
    private readonly Query<User> _query;
    public GetUsersByEmailHandler(Query<User> query) => _query = query;
    public Task<IEnumerable<User>> Handle(GetUsersByEmail request, CancellationToken ct) => _query.GetByFilter(u => u.Email == request.Email);
}
/*---------------------------------------------------------------------------------------------------------*/

/*--------------------------------------------------commands--------------------------------------------------*/
/** Implements the IRequestHandler interface for the CreateUser request. */
public sealed class CreateUserHandler : IRequestHandler<CreateUser, User>
{
    private readonly Mutation<User> _mutation;
    public CreateUserHandler(Mutation<User> mutation) => _mutation = mutation;
    public async Task<User> Handle(CreateUser request, CancellationToken cancellationToken) => await _mutation.Create(request.Input);
}

/** Implements the IRequestHandler interface for the UpdateUser request. */
public sealed class UpdateUserHandler : IRequestHandler<UpdateUser, User>
{
    private readonly Mutation<User> _mutation;
    public UpdateUserHandler(Mutation<User> mutation) => _mutation = mutation;
    public async Task<User> Handle(UpdateUser request, CancellationToken cancellationToken) => await _mutation.Update(request.Id, request.Input);
}

/** Implements the IRequestHandler interface for the DeleteUser request. */
public sealed class DeleteUserHandler : IRequestHandler<DeleteUser, bool>
{
    private readonly Mutation<User> _mutation;
    public DeleteUserHandler(Mutation<User> mutation) => _mutation = mutation;
    public Task<bool> Handle(DeleteUser request, CancellationToken cancellationToken) => _mutation.Delete(request.Id);
}

/** User-specific CQRS records that wrap the generic primitives. */
#region Records ----------------------------------------------------------------

public sealed record GetUsers() : GetAllQuery<User>();
public sealed record GetUserById(int Id) : GetByIdQuery<User>(Id);
public sealed record GetUsersByEmail(string Email) : GetByFilterQuery<User>(p => p.Email == Email);

public sealed record CreateUser(UserDto Input) : CreateCommand<UserDto, User>(Input);
public sealed record UpdateUser(int Id, UserDto Input) : UpdateCommand<UserDto, User>(Id, Input);
public sealed record DeleteUser(int Id) : DeleteCommand<User>(Id);

#endregion --------------------------------------------------------------------
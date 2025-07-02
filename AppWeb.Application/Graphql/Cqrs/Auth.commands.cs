using AppWeb.Application.Graphql.Handlers;
using AppWeb.Shared.Dtos;
using MediatR;

namespace AppWeb.Application.Graphql.Cqrs;

[ExtendObjectType("Mutation")]
public class AuthCommand
{
    /**
     * Login a user
     * Pass a dictionary with the fields to create
     * e.g., new Dictionary<string, object> { { "Name", "New Name" }, { "Age", 30 } }
     */
    public async Task<LoginResultDto> Login([Service] IMediator mediator, LoginDto dto) => await mediator.Send(new LoginCommand(dto));
}
using AppWeb.Application.Graphql.Handlers;
using AppWeb.Models;
using MediatR;

namespace AppWeb.Application.Graphql.Queries;

[ExtendObjectType("Query")]
public class UserQuery
{
    /**
     * Retrieves all users from the database.
     * @return A list of all users.
     */
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<User>> GetUsers([Service] IMediator mediator) => await mediator.Send(new GetUsers());

    /**
     * Retrieves a user by their ID.
     * @param id The ID of the user to retrieve.
     * @return The user with the specified ID, or null if not found.
     */
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<User?> GetUserById([Service] IMediator mediator, int id) => await mediator.Send(new GetUserById(id));

    /**
     * Retrieves users by their email address.
     * @param email The email address of the users to retrieve.
     * @return A list of users with the specified email address.
     */
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<User>> GetUsersByEmail([Service] IMediator mediator, string email) => await mediator.Send(new GetUsersByEmail(email));
}
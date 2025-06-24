using AppWeb.Models;

namespace AppWeb.Application.Interfaces.Persistence;

/// <summary>
/// Repository contract for <see cref="User"/> aggregate.
/// Extends the generic <see cref="IRepository{TEntity}"/> abstraction.
/// </summary>
public interface IUserRepository : IRepository<User> { }
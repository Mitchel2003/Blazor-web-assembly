using AppWeb.Models;

namespace AppWeb.Application.Contracts.Persistence.Repositories;

/// <summary>
/// Repository contract for <see cref="User"/> aggregate.
/// Extends the generic <see cref="IRepository{TEntity}"/> abstraction.
/// </summary>
public interface IUserRepository : IRepository<User> { }
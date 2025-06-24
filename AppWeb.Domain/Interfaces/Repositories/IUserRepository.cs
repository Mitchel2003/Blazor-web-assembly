using AppWeb.Domain.Models;

namespace AppWeb.Domain.Interfaces;

/// <summary>
/// Repository contract for <see cref="User"/> aggregate.
/// Extends the generic <see cref="IRepository{TEntity}"/> abstraction.
/// </summary>
public interface IUserRepository : IRepository<User> { }
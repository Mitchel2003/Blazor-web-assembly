using AppWeb.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using AppWeb.Models;

namespace AppWeb.Infrastructure.Persistence.Repositories;

/// <summary>
/// Concrete repository implementation for <see cref="User"/> aggregate.
/// Inherits all generic behavior and exposes it via <see cref="IUserRepository"/>.
/// </summary>
internal sealed class UserRepository : Repository<User>, IUserRepository
{ public UserRepository(IDbContextFactory<AppDBContext> factory) : base(factory) { } }
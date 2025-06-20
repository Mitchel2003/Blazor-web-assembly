using Microsoft.EntityFrameworkCore;
using AppWeb.Models;

namespace AppWeb.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core DbContext for AppWeb.
/// Placed in the Infrastructure layer to keep Domain pure per Clean Architecture guidelines.
/// </summary>
public class AppDBContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Applies all IEntityTypeConfiguration<T> found in this assembly (e.g., UserConfiguration)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDBContext).Assembly);
    }
}
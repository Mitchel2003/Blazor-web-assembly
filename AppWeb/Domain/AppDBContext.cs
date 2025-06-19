using Microsoft.EntityFrameworkCore;
using AppWeb.Models;

namespace AppWeb.Domain;

public class AppDBContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted); }
}
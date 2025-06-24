using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AppWeb.Domain.Models;

namespace AppWeb.Infrastructure.Persistence.Configurations;

/// <summary> Fluent API configuration for <see cref="User"/> entity. Keeps DbContext clean and adheres to SRP. </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        // Global soft-delete filter
        builder.HasQueryFilter(u => !u.IsDeleted);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
    }
}
using Microsoft.EntityFrameworkCore;
using AppWeb.Domain.Interfaces;
using System.Linq.Expressions;
using Mapster;

namespace AppWeb.Infrastructure.Persistence;

/// <summary>
/// Generic repository implementation backed by EF Core.
/// All interactions are executed using a pooled factory to keep DbContext short-lived.
/// </summary>
internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
{
    private readonly IDbContextFactory<AppDBContext> _context;
    public Repository(IDbContextFactory<AppDBContext> context) => _context = context;

    #region Queries ------------------------------------------------------------    
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        await using var db = await _context.CreateDbContextAsync();
        return await db.Set<TEntity>().AsNoTracking().ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        await using var db = await _context.CreateDbContextAsync();
        return await db.FindAsync<TEntity>(id);
    }

    public async Task<IEnumerable<TEntity>> GetByFilterAsync(Expression<Func<TEntity, bool>> filter)
    {
        await using var db = await _context.CreateDbContextAsync();
        return await db.Set<TEntity>().AsNoTracking().Where(filter).ToListAsync();
    }
    #endregion ---------------------------------------------------------------------

    #region Mutations ------------------------------------------------------------    
    public async Task<TEntity> CreateAsync<TInput>(TInput dto) where TInput : class
    {
        var entity = dto.Adapt<TEntity>();
        await using var db = await _context.CreateDbContextAsync();
        db.Set<TEntity>().Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> UpdateAsync<TInput>(int id, TInput dto) where TInput : class
    {
        await using var db = await _context.CreateDbContextAsync();
        var entity = await db.FindAsync<TEntity>(id) ?? throw new KeyNotFoundException("Entity not found");
        dto.Adapt(entity);
        db.Update(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var db = await _context.CreateDbContextAsync();
        var entity = await db.FindAsync<TEntity>(id);
        if (entity is null) return false;

        // Soft delete by setting IsActive to false if it exists; otherwise hard delete
        var prop = typeof(TEntity).GetProperty("IsActive", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
        if (prop != null && prop.PropertyType == typeof(bool)) { prop.SetValue(entity, false); db.Update(entity); }
        else { db.Remove(entity); }
        await db.SaveChangesAsync();
        return true;
    }
}
#endregion ---------------------------------------------------------------------
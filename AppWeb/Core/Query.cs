using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AppWeb.Domain;

namespace AppWeb.Core;
public class Query<T> where T : class
{
    private readonly IDbContextFactory<AppDBContext> _factory;
    public Query([Service] IDbContextFactory<AppDBContext> factory) { _factory = factory; }

    /**
     * Retrieves all entities of type T from the database.
     * @return A list of all entities of type T.
     */
    public async Task<IEnumerable<T>> GetAll()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Set<T>().ToListAsync();
    }

    /**
     * Retrieves an entity of type T by its ID.
     * @param id The ID of the entity to retrieve.
     * @return The entity of type T with the specified ID, or null if not found.
     */
    public async Task<T?> GetById(object id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.FindAsync<T>(id);
    }

    /**
     * Retrieves entities of type T that match the specified filter.
     * @param filter The filter expression to apply to the query.
     * @return A list of entities of type T that match the filter.
     */
    public async Task<IEnumerable<T>> GetByFilter(Expression<Func<T, bool>> filter)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Set<T>().Where(filter).ToListAsync();
    }
}
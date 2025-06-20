using AppWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace AppWeb.Core;

public class Mutation<T> where T : class, new()
{
    private readonly IDbContextFactory<AppDBContext> _factory;
    private static readonly Func<T> _instance = CreateInstanceFactory();
    public Mutation([Service] IDbContextFactory<AppDBContext> factory) { _factory = factory; }

    /**
     * Create a new entity of the specified model type.
     * Pass a dictionary with the fields to create.
     * e.g., new Dictionary<string, object> { { "Name", "New Name" }, { "Age", 30 } }
     */
    public async Task<T> Create<TInput>(TInput dto) where TInput : class
    {
        T instance = _instance();
        ApplyProperties(instance, dto);

        await using var db = await _factory.CreateDbContextAsync();
        db.Set<T>().Add(instance);
        await db.SaveChangesAsync();
        return instance;
    }

    /**
     * Update an existing entity by ID.
     * If you want to update only specific fields, pass a dictionary with the fields to update.
     * e.g., new Dictionary<string, object> { { "Name", "New Name" }, { "Age", 30 } }
     */
    public async Task<T> Update<TInput>(object id, TInput dto) where TInput : class
    {
        await using var db = await _factory.CreateDbContextAsync();
        var entity = await db.FindAsync<T>(id) ?? throw new GraphQLException("Entity not found.");

        ApplyProperties(entity, dto);
        db.Update(entity); //onUpdate
        await db.SaveChangesAsync();
        return entity;
    }

    /**
     * Soft delete by setting IsDeleted = true.
     * If you want to hard delete, you can implement a different method.
     * This method assumes the model has a property named "IsDeleted" of type bool.
     */
    public async Task<bool> Delete(object id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var entity = await db.FindAsync<T>(id) ?? throw new GraphQLException("Entity not found.");

        var prop = typeof(T).GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null || prop.PropertyType != typeof(bool)) throw new GraphQLException("Soft delete not supported for this model.");

        prop.SetValue(entity, true);
        db.Update(entity); //onUpdate
        await db.SaveChangesAsync();
        return true;
    }
    /*---------------------------------------------------------------------------------------------------------*/

    /*--------------------------------------------------tools--------------------------------------------------*/
    /**
     * Creates a factory method to instantiate T using Expression Trees.
     * This is more efficient than Activator.CreateInstance<T>() for performance-critical scenarios.
     * We avoid reflection at runtime by compiling the expression tree that has been delegate.
     */
    private static Func<T> CreateInstanceFactory()
    {
        var ctor = Expression.New(typeof(T));
        var lambda = Expression.Lambda<Func<T>>(ctor);
        return lambda.Compile();
    }

    /**
     * Use reflection to map properties from a DTO to the entity
     * This method assumes that the DTO has properties that match the entity's properties.
     * This focus is perfect when working with GraphQL and we want to map DTOs to entities dynamically.
     */
    private static void ApplyProperties<TInput>(T target, TInput source) where TInput : class
    { //Determine the properties of the source type; in other words, the DTO. REMEMBER IT...
        var props = typeof(TInput).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var item in props)
        { //Use reflection to set properties dynamically, checking type of property existing in the target
            var type = typeof(T).GetProperty(item.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (type != null && type.CanWrite)
            { //Property exists and is writable
                var value = item.GetValue(source);
                if (value != null) type.SetValue(target, Convert.ChangeType(value, type.PropertyType));
            }
        }
    }
}
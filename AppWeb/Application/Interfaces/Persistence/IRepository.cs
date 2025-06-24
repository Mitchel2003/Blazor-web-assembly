using System.Linq.Expressions;

namespace AppWeb.Application.Interfaces.Persistence;

/// <summary> Generic repository abstraction for aggregate roots. </summary>
public interface IRepository<TEntity> where TEntity : class, new()
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetByFilterAsync(Expression<Func<TEntity,bool>> filter);
    Task<TEntity> CreateAsync<TInput>(TInput dto) where TInput : class;
    Task<TEntity> UpdateAsync<TInput>(int id, TInput dto) where TInput : class;
    Task<bool> DeleteAsync(int id);
}
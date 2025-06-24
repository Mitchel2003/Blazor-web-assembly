using System.Linq.Expressions;
using MediatR;

namespace AppWeb.Server.Application.Core;

/*
 * Generic CQRS primitives (Commands, Queries)
 * for free by simply deriving concrete commands/queries from these records.
 */
#region Queries ------------------------------------------------------------
public record GetAllQuery<TEntity>() : IRequest<IEnumerable<TEntity>>
    where TEntity : class, new();

public record GetByIdQuery<TEntity>(int Id) : IRequest<TEntity?>
    where TEntity : class, new();

public record GetByFilterQuery<TEntity>(Expression<Func<TEntity, bool>> Filter) : IRequest<IEnumerable<TEntity>>
    where TEntity : class, new();
#endregion ---------------------------------------------------------------------

#region Commands ------------------------------------------------------------
public abstract record CreateCommand<TInput, TEntity>(TInput Input) : IRequest<TEntity>
    where TEntity : class, new()
    where TInput  : class;

public abstract record UpdateCommand<TInput, TEntity>(int Id, TInput Input) : IRequest<TEntity>
    where TEntity : class, new()
    where TInput  : class;

public record DeleteCommand<TEntity>(int Id) : IRequest<bool>
    where TEntity : class, new();
#endregion ---------------------------------------------------------------------
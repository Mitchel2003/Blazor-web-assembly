using FluentValidation;
using MediatR;

namespace AppWeb.Server.Application.Core;

/// <summary>
/// MediatR pipeline behavior that executes all FluentValidation validators associated
/// with the current <typeparamref name="TRequest"/>. Validation failures throw a <see cref="ValidationException"/>,
/// which can be mapped to a problem-details response by upper layers.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)))).SelectMany(r => r.Errors).Where(f => f != null).ToList();
            if (failures.Count != 0) throw new ValidationException(failures);
        }
        return await next();
    }
}
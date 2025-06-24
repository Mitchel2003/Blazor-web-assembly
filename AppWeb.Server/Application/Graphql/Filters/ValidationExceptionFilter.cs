using FluentValidation;

namespace AppWeb.Server.Application.Graphql.Filters;

/// <summary>
/// Transforms <see cref="ValidationException"/> thrown by FluentValidation pipeline
/// into a GraphQL error that the client can parse and surface gracefully.
/// </summary>
public sealed class ValidationExceptionFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        if (error.Exception is ValidationException valEx)
        { //Aggregate validation messages into a single, user-friendly string.
            var messages = string.Join(" | ", valEx.Errors.Select(f => f.ErrorMessage));
            return ErrorBuilder.New().SetMessage(messages).SetCode("VALIDATION_ERROR").Build();
        }
        return error;
    }
}
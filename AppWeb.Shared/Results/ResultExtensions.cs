namespace AppWeb.Shared.Results;

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map)
    {
        return result.IsSuccess && result.Value is not null
            ? Result<TOut>.Success(map(result.Value))
            : Result<TOut>.Failure(result.Error ?? $"Error inesperado - ${result.Error}");
    }
}
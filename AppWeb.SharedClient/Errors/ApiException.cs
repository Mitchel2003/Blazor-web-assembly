using AppWeb.SharedClient.Http;
using System.Net;

namespace AppWeb.SharedClient.Errors;

public class ApiException : Exception
{
    private static readonly IApiErrorParser _errorParser = new ApiErrorParser();
    
    public HttpStatusCode StatusCode { get; }
    public IReadOnlyList<string> Errors { get; }
    public ApiException(HttpStatusCode statusCode, IEnumerable<string> errors) : base(string.Join(" | ", errors))
    { StatusCode = statusCode; Errors = errors.ToList(); }

    /// <summary>
    /// Factory helper that converts an <see cref="HttpResponseMessage"/> into a rich <see cref="ApiException"/>,
    /// centralising the error-parsing logic for reuse across every API client.
    /// </summary>
    public static async Task<ApiException> FromHttpResponseAsync(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadAsStringAsync();
        var errors = _errorParser.ParseErrors(payload);
        return new ApiException(response.StatusCode, errors);
    }
}
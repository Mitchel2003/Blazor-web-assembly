using AppWeb.Client.Http;
using System.Net;

namespace AppWeb.Client.Errors;

public class ApiException : Exception
{
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
        var errors = ApiErrorParser.ParseErrors(payload);
        return new ApiException(response.StatusCode, errors);
    }
}
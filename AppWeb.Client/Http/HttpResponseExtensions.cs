using AppWeb.Client.Errors;
using System.Text.Json;
using System.Net;

namespace AppWeb.Client.Http;

/// <summary> Provides shared, reusable extension methods for handling HttpResponseMessage instances. </summary>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Centralized handler for API responses. It checks for unsuccessful HTTP status codes
    /// and also inspects the JSON payload for GraphQL-specific errors, even on HTTP 200 OK responses.
    /// Throws an <see cref="ApiException"/> if any errors are found.
    /// </summary>
    /// <returns>The raw JSON payload as a string if the response is successful.</returns>
    public static async Task<string> EnsureGraphQLSuccessAsync(this HttpResponseMessage response, CancellationToken ct = default)
    {
        var json = await response.Content.ReadAsStringAsync(ct); // Read the response content as a string
        if (!response.IsSuccessStatusCode) throw new ApiException(response.StatusCode, ApiErrorParser.ParseErrors(json));
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("errors", out var errs) && errs.ValueKind == JsonValueKind.Array && errs.GetArrayLength() > 0)
            { throw new ApiException(HttpStatusCode.OK, ApiErrorParser.ParseErrors(json)); }
        }
        catch (JsonException)
        {
            //If the response is not valid JSON, we can't parse it for GraphQL errors.
            //Since the status code was successful, we proceed, returning the raw content.
        }
        return json;
    }
}
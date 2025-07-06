using System.Text.Json;

namespace AppWeb.Client.Http;

/// <summary>Helper utilities to transform raw HTTP/GraphQL error payloads into user-friendly messages</summary>
public class ApiErrorParser : IApiErrorParser
{
    /// <summary>
    /// Parses a JSON payload that follows the GraphQL spec { "errors": [ { "message": "..." } ] }.
    /// If the payload cannot be parsed, it is returned verbatim in a single-item array.
    /// </summary>
    public IEnumerable<string> ParseErrors(string errorJson)
    {
        if (string.IsNullOrWhiteSpace(errorJson)) return new[] { "Unknown error" };
        try
        {
            using var doc = JsonDocument.Parse(errorJson);
            if (doc.RootElement.TryGetProperty("errors", out var errorsElement) && errorsElement.ValueKind == JsonValueKind.Array)
            { return errorsElement.EnumerateArray().Select(e => e.GetProperty("message").GetString() ?? "Error").ToArray(); }
        }
        catch (JsonException) { /* Fall back to raw string below. */ }
        return new[] { errorJson };
    }
}

#region Interface ------------------------------------------------------------
public interface IApiErrorParser
{
    /// <summary>
    /// Parses a JSON payload that follows the GraphQL spec { "errors": [ { "message": "..." } ] }.
    /// If the payload cannot be parsed, it is returned verbatim in a single-item array.
    /// </summary>
    IEnumerable<string> ParseErrors(string errorJson);
}
#endregion ---------------------------------------------------------------------
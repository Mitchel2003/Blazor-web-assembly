using AppWeb.SharedClient.Errors;
using MudBlazor;

namespace AppWeb.Client.Errors;

/// <summary>
/// Emits system-wide error notifications leveraging MudBlazor's Snackbar service.
/// Components may inject this service to surface exceptions in a consistent, UX-friendly manner.
/// </summary>
public class ErrorNotifier
{
    private readonly ISnackbar _snackbar;
    public event Action<string>? ErrorRaised;
    public ErrorNotifier(ISnackbar snackbar) => _snackbar = snackbar;
    public void Notify(Exception ex)
    {
        var message = ex switch { ApiException api => string.Join("\n", api.Errors), _ => ex.Message };
        _snackbar.Add(message, Severity.Error);
        ErrorRaised?.Invoke(message);
    }
}
using AppWeb.ViewModels.Core.Services;
using MudBlazor;

namespace AppWeb.Client.Services.Blazor;

public class BlazorMessageService : IMessageService
{
    private readonly IDialogService _dialogService;
    private readonly ISnackbar _snackbar;

    public BlazorMessageService(ISnackbar snackbar, IDialogService dialogService)
    { _snackbar = snackbar; _dialogService = dialogService; }

    public Task ShowErrorAsync(string message)
    {
        _snackbar.Add(message, Severity.Error);
        return Task.CompletedTask;
    }

    public Task ShowSuccessAsync(string message)
    {
        _snackbar.Add(message, Severity.Success);
        return Task.CompletedTask;
    }

    public async Task<bool> ConfirmAsync(string title, string message, string acceptText = "Aceptar", string cancelText = "Cancelar")
    {
        var result = await _dialogService.ShowMessageBox(title, message, acceptText, cancelText);
        return result == true;
    }
}
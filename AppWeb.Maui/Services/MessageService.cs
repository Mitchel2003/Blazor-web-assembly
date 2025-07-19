using AppWeb.Shared.Services.Contracts;

namespace AppWeb.Maui.Services;

/// <summary>
/// MAUI implementation of the IMessageService defined in the ViewModels project.
/// Provides UI alert and dialog functionality for MAUI.
/// </summary>
public class MessageService : IMessageService
{
    /// <summary>Muestra un mensaje de error.</summary>
    public async Task ShowErrorAsync(string message)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        { await Application.Current.MainPage.DisplayAlert("Error", message, "OK"); });
    }

    /// <summary>Muestra un mensaje de éxito.</summary>
    public async Task ShowSuccessAsync(string message)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        { await Application.Current.MainPage.DisplayAlert("Success", message, "OK"); });
    }

    /// <summary>Muestra un diálogo de confirmación.</summary>
    public async Task<bool> ConfirmAsync(string title, string message, string acceptText = "Aceptar", string cancelText = "Cancelar")
    { return await Application.Current.MainPage.DisplayAlert(title, message, acceptText, cancelText); }

    // Additional MAUI-specific methods that extend the base interface functionality

    /// <summary>Shows an action sheet with multiple options.</summary>
    public async Task<string> ShowActionSheetAsync(string title, string cancelText, string destructionText, params string[] buttons)
    { return await Application.Current.MainPage.DisplayActionSheet(title, cancelText, destructionText, buttons); }

    /// <summary>Shows a prompt for user input.</summary>
    public async Task<string> ShowPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string? placeholder = "", int maxLength = -1)
    { return await Application.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength); }

    public void ShowToast(string message, int durationMilliseconds = 3000)
    {
        // Using MAUI's built-in Toast notification system
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            // TODO: Implement toast with CommunityToolkit.Maui or other method
            // For now, we'll use an alert with auto-dismiss
            var page = Application.Current.MainPage;
            await page.DisplayAlert("Notification", message, "OK");
        });
    }
}
namespace AppWeb.Shared.Services.Contracts;

/// <summary>Service to show messages to the user.</summary>
public interface IMessageService
{
    /// <summary>Show an error message.</summary>
    Task ShowErrorAsync(string message);

    /// <summary>Show a success message.</summary>
    Task ShowSuccessAsync(string message);

    /// <summary>Show a confirmation dialog.</summary>
    Task<bool> ConfirmAsync(string title, string message, string acceptText = "Aceptar", string cancelText = "Cancelar");
}
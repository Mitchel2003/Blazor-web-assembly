namespace AppWeb.ViewModels.Core.Services;

/// <summary>Servicio para mostrar mensajes al usuario.</summary>
public interface IMessageService
{
    /// <summary>Muestra un mensaje de error.</summary>
    Task ShowErrorAsync(string message);

    /// <summary>Muestra un mensaje de éxito.</summary>
    Task ShowSuccessAsync(string message);

    /// <summary>Muestra un diálogo de confirmación.</summary>
    Task<bool> ConfirmAsync(string title, string message, string acceptText = "Aceptar", string cancelText = "Cancelar");
}
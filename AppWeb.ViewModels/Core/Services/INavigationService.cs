namespace AppWeb.ViewModels.Core.Services;

/// <summary>Servicio para navegación entre pantallas.</summary>
public interface INavigationService
{
    /// <summary>Navega hacia atrás en la pila de navegación.</summary>
    Task GoBackAsync();

    /// <summary>Navega a una ruta específica con parámetros opcionales.</summary>
    Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null);
}
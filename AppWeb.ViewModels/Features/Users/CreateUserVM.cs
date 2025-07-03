using CommunityToolkit.Mvvm.ComponentModel;
using AppWeb.ViewModels.Core.Services;
using AppWeb.ViewModels.Core.Base;
using AppWeb.Shared.Inputs;

namespace AppWeb.ViewModels.Features.Users;

/// <summary>
/// ViewModel para la creación de usuarios.
/// Mantiene responsabilidad única para el caso de uso de creación.
/// </summary>
public partial class CreateUserVM : ViewModelBase, ICreateUserVM
{
    private readonly INavigationService _navigationService;
    private readonly IMessageService _messageService;
    private readonly IUsersService _usersService;

    [ObservableProperty] private CreateUserInput input = new();
    [ObservableProperty] private bool saveSuccess;
    [ObservableProperty] private bool isSaving;

    public CreateUserVM(IUsersService usersService, INavigationService navigationService, IMessageService messageService)
    {
        _usersService = usersService;
        _navigationService = navigationService;
        _messageService = messageService;
    }

    /// <summary>Ejecuta el proceso de creación de usuario.</summary>
    public async Task<bool> CreateUserAsync(CancellationToken cancellationToken = default)
    {
        if (IsSaving) return false;
        try
        {
            IsSaving = true;
            SaveSuccess = false;
            var result = await _usersService.CreateUserAsync(Input, cancellationToken);
            if (result == null) { await _messageService.ShowErrorAsync("Error al crear usuario"); return false; }
            await _messageService.ShowSuccessAsync("Usuario creado exitosamente");
            SaveSuccess = true;
            
            //Delay to allow showing success message
            await Task.Delay(1500, cancellationToken);
            await _navigationService.NavigateToAsync("/users");
            return true;
        }
        catch (Exception ex) { await _messageService.ShowErrorAsync($"Error inesperado: {ex.Message}"); return false; }
        finally { IsSaving = false; }
    }
}

#region Interfaces ------------------------------------------------------------
public interface ICreateUserVM
{
    /// <summary>Entrada de datos para crear un usuario.</summary>
    CreateUserInput Input { get; }

    /// <summary>Indica si el proceso de guardado está en curso.</summary>
    bool IsSaving { get; }

    /// <summary>Indica si la operación se completó exitosamente.</summary>
    bool SaveSuccess { get; }

    /// <summary>Ejecuta la creación del usuario con los datos del Input.</summary>
    Task<bool> CreateUserAsync(CancellationToken cancellationToken = default);
}
#endregion ---------------------------------------------------------------------
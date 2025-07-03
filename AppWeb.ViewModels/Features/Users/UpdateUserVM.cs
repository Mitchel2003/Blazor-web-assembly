using CommunityToolkit.Mvvm.ComponentModel;
using AppWeb.ViewModels.Core.Services;
using AppWeb.ViewModels.Core.Base;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;

namespace AppWeb.ViewModels.Features.Users;

/// <summary>
/// ViewModel para la actualización de usuarios.
/// Mantiene responsabilidad única para el caso de uso de actualización.
/// </summary>
public partial class UpdateUserVM : ViewModelBase, IUpdateUserVM
{
    private readonly INavigationService _navigationService;
    private readonly IMessageService _messageService;
    private readonly IUsersService _usersService;

    [ObservableProperty] private UpdateUserInput input = new();
    [ObservableProperty] private UserResultDto? existing;
    [ObservableProperty] private bool saveSuccess;
    [ObservableProperty] private bool isSaving;
    [ObservableProperty] private bool isLoading;

    public UpdateUserVM(IUsersService usersService, INavigationService navigationService, IMessageService messageService)
    {
        _usersService = usersService;
        _navigationService = navigationService;
        _messageService = messageService;
    }

    /// <summary>Renderiza el formulario de actualización.</summary>
    public async Task<bool> LoadUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (IsLoading) return false;
        try
        {
            IsLoading = true;
            var user = await _usersService.GetUserByIdAsync(userId, cancellationToken);
            if (user == null) { await _messageService.ShowErrorAsync("Usuario no encontrado"); return false; }
            
            Existing = user;
            Input = new UpdateUserInput
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Password = user.Password,
                IsActive = user.IsActive
            };
            return true;
        }
        catch (Exception ex) { await _messageService.ShowErrorAsync($"Error inesperado: {ex.Message}"); return false; }
        finally { IsLoading = false; }
    }

    /// <summary>Ejecuta el proceso de actualización de usuario.</summary>
    public async Task<bool> UpdateUserAsync(CancellationToken cancellationToken = default)
    {
        if (IsSaving) return false;
        try
        {
            IsSaving = true;
            SaveSuccess = false;
            var result = await _usersService.UpdateUserAsync(Input, cancellationToken);
            if (result == null) { await _messageService.ShowErrorAsync("Error al actualizar usuario"); return false; }
            await _messageService.ShowSuccessAsync("Usuario actualizado exitosamente");
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
public interface IUpdateUserVM
{
    /// <summary>Datos del usuario existente.</summary>
    UserResultDto? Existing { get; }

    /// <summary>Entrada de datos para actualizar el usuario.</summary>
    UpdateUserInput Input { get; }

    /// <summary>Indica si está cargando los datos del usuario.</summary>
    bool IsLoading { get; }

    /// <summary>Indica si el proceso de guardado está en curso.</summary>
    bool IsSaving { get; }

    /// <summary>Indica si la operación se completó exitosamente.</summary>
    bool SaveSuccess { get; }

    /// <summary>Carga los datos del usuario a editar.</summary>
    Task<bool> LoadUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>Ejecuta la actualización del usuario con los datos del Input.</summary>
    Task<bool> UpdateUserAsync(CancellationToken cancellationToken = default);
}
#endregion ---------------------------------------------------------------------
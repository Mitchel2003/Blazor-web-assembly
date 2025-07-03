using CommunityToolkit.Mvvm.ComponentModel;

namespace AppWeb.ViewModels.Core.Base;

/// <summary>Base abstracta para ViewModels que implementan operaciones CRUD.</summary>
public abstract partial class ViewModelCrud<TModel, TId> : ViewModelBase
{
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private bool operationSuccess;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private bool isSaving;

    /// <summary>
    /// Obtiene un elemento por su identificador.
    /// </summary>
    public abstract Task<TModel?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>Guarda los cambios (creación o actualización).</summary>
    public abstract Task<bool> SaveAsync(CancellationToken cancellationToken = default);
}
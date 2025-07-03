using CommunityToolkit.Mvvm.ComponentModel;

namespace AppWeb.ViewModels.Core.Base;

/// <summary>Base para todos los ViewModels con funcionalidades comunes.</summary>
public abstract class ViewModelBase : ObservableObject, IDisposable
{
    /// <summary>Libera recursos no administrados.</summary>
    public virtual void Dispose() { }

    /// <summary>Inicializa el ViewModel con los datos necesarios.</summary>
    public virtual Task InitializeAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
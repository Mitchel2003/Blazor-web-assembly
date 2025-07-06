using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;

namespace AppWeb.ViewModels.Core.Base;

/// <summary>Enhanced base class for all ViewModels with comprehensive functionality.</summary>
public abstract partial class ViewModelBase : ObservableObject, IDisposable
{
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private bool _isInitialized;
    [ObservableProperty] private bool _isBusy;
    private bool _isDisposed;

    //Command factories for cleaner implementation
    protected AsyncRelayCommand CreateCommand(Func<Task> execute) => new AsyncRelayCommand(execute, () => !IsBusy);
    protected AsyncRelayCommand<T> CreateCommand<T>(Func<T, Task> execute) => new AsyncRelayCommand<T>(execute!, _ => !IsBusy);

    //Enhanced property change notification
    protected bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null!)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
        storage = value; onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>Initializes the ViewModel with the data needed for display.</summary>
    public virtual async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized) return;
        try { IsBusy = true; await OnInitializeAsync(cancellationToken); IsInitialized = true; }
        catch (Exception ex) { ErrorMessage = ex.Message; System.Diagnostics.Debug.WriteLine($"ViewModel initialization failed: {ex}"); }
        finally { IsBusy = false; }
    }

    #region Helpers ------------------------------------------------------------

    /// <summary>Releases all resources used by the ViewModel.</summary>
    public virtual void Dispose()
    {
        if (_isDisposed) return;
        OnDispose(true); _isDisposed = true;
        GC.SuppressFinalize(this); //claan memory
    }

    /// <summary>Override to implement custom disposal logic.</summary>
    protected virtual void OnDispose(bool disposing) { } //not implemented
    /// <summary>Override this method to perform custom initialization logic.</summary>
    protected virtual Task OnInitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    #endregion ---------------------------------------------------------------------
}
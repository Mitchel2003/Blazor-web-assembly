using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;

namespace AppWeb.ViewModels.Core.Base;

/// <summary>Enhanced base class for all ViewModels with comprehensive functionality.</summary>
public abstract partial class ViewModelBase : ObservableObject, IViewModelBase
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

    // Navigation lifecycle methods for MAUI compatibility
    /// <summary>Called when navigating to a page with parameters.</summary>
    public virtual Task OnNavigatingToAsync(object parameter) => Task.CompletedTask;

    /// <summary>Called when navigated to a page.</summary>
    public virtual Task OnNavigatedToAsync() => Task.CompletedTask;

    /// <summary>Called when navigating away from a page.</summary>
    public virtual Task OnNavigatedFromAsync() => Task.CompletedTask;

    /// <summary>Called when the page is disappearing.</summary>
    public virtual Task OnDisappearing() => Task.CompletedTask;

    #region Helpers ------------------------------------------------------------
    /// <summary>Releases all resources used by the ViewModel.</summary>
    public virtual void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        OnDispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>Override to release resources during disposal.</summary>
    protected virtual void OnDispose() { }

    /// <summary>Initialization logic for the ViewModel.</summary>
    protected virtual Task OnInitializeAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    #endregion ---------------------------------------------------------------------
}

#region Interfaces ------------------------------------------------------------
/// <summary>Interface for all ViewModels with core functionality.</summary>
public interface IViewModelBase : IDisposable
{
    /// <summary>Gets or sets error message.</summary>
    string ErrorMessage { get; set; }
    
    /// <summary>Gets or sets the title of the ViewModel.</summary>
    string Title { get; set; }
    
    /// <summary>Gets or sets a value indicating whether the ViewModel is initialized.</summary>
    bool IsInitialized { get; set; }
    
    /// <summary>Gets or sets a value indicating whether the ViewModel is busy.</summary>
    bool IsBusy { get; set; }
    
    /// <summary>Initializes the ViewModel with the data needed for display.</summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);
    
    /// <summary>Called when navigating to a page with parameters.</summary>
    Task OnNavigatingToAsync(object parameter);
    
    /// <summary>Called when navigated to a page.</summary>
    Task OnNavigatedToAsync();
    
    /// <summary>Called when navigating away from a page.</summary>
    Task OnNavigatedFromAsync();
    
    /// <summary>Called when the page is disappearing.</summary>
    Task OnDisappearing();
}
#endregion ---------------------------------------------------------------------
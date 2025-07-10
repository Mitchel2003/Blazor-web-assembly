using CommunityToolkit.Mvvm.ComponentModel;
using AppWeb.Shared.Services.Contracts;
using AppWeb.ViewModels.Core.Factory;
using CommunityToolkit.Mvvm.Input;

namespace AppWeb.ViewModels.Core.Base;

/// <summary>
/// Base class for ViewModels that implement CRUD operations.
/// TModel is the model type for input, TIdType is the type of the model's ID.
/// </summary>
public abstract partial class ViewModelCrud<TModel, TIdType> : ValidatableViewModel, IViewModelCrud<TModel, TIdType>
    where TModel : class, new()
    where TIdType : struct
{
    [ObservableProperty] protected bool _operationSuccess;
    [ObservableProperty] protected bool _isModified;
    [ObservableProperty] protected bool _isLoading;
    [ObservableProperty] protected bool _isSaving;
    [ObservableProperty] protected bool _isNew;
    
    [ObservableProperty] protected TIdType _modelId;
    [ObservableProperty] protected TModel _model;
    
    protected readonly IModelFactory _modelFactory;
    
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand ValidateCommand { get; }
    
    protected ViewModelCrud(IModelFactory modelFactory, IMessageService? messageService = null)
        : base(messageService)
    {
        _modelFactory = modelFactory;
        Model = _modelFactory.Create<TModel>();
        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        ValidateCommand = new AsyncRelayCommand(ValidateModelAsync);
    }

    /// <summary>Called after model is loaded successfully.</summary>
    protected virtual void OnModelLoaded() { } //not implemented yet
    
    /// <summary>Validates the current model and shows validation errors.</summary>
    private async Task ValidateModelAsync()
    {
        await ValidateAsync(); //validate the model
        await ShowMessageErrorValidationAsync();
    }

    /// <summary>Initializes the ViewModel with default or provided values.</summary>
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await base.InitializeAsync(cancellationToken);
        IsModified = false; OperationSuccess = false;
    }
    
    /// <summary>Initializes the ViewModel with a specific entity ID.</summary>
    /// <param name="id">The ID of the entity to load.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual async Task InitializeWithIdAsync(TIdType id, CancellationToken cancellationToken = default)
    {
        ModelId = id;
        await InitializeAsync(cancellationToken);
        await LoadByIdAsync(id, cancellationToken);
    }
    
    /// <summary>Initializes the ViewModel for creating a new entity.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual async Task InitializeForNewAsync(CancellationToken cancellationToken = default)
    {
        IsNew = true;
        Model = _modelFactory.Create<TModel>();
        await InitializeAsync(cancellationToken);
    }
    
    /// <summary>Loads the model by ID.</summary>
    public async Task LoadByIdAsync(TIdType id, CancellationToken cancellationToken = default)
    {
        if (IsLoading) return;
        try
        {
            IsLoading = true;
            var model = await GetByIdAsync(id, cancellationToken);
            if (model != null) //if model is not null, set state and call the OnModelLoaded method
            { Model = model; ModelId = id; IsNew = false; IsModified = false; OnModelLoaded(); }
        }
        finally { IsLoading = false; }
    }
    
    /// <summary>Saves the model (create or update).</summary>
    public async Task<bool> SaveAsync()
    {
        if (IsSaving) return false;
        try
        {
            if (!await ValidateAsync()) { await ShowMessageErrorValidationAsync(); return false; }
            IsSaving = true; //set the saving state to true
            OperationSuccess = await OnSaveAsync();
            return OperationSuccess;
        }
        finally { IsSaving = false; }
    }
    
    /// <summary>Determines if saving is allowed.</summary>
    private bool CanSave() => IsModified && !IsSaving;

    /// <summary>Implementation for saving a model. Must be implemented by derived classes.</summary>
    protected abstract Task<bool> OnSaveAsync();

    /// <summary>Gets the model by ID. Must be implemented by derived classes.</summary>
    public abstract Task<TModel> GetByIdAsync(TIdType id, CancellationToken cancellationToken = default);
    
    /// <summary>Resets the model to a new instance.</summary>
    protected virtual void ResetModel()
    {
        Model = _modelFactory.Create<TModel>();
        ModelId = default;
        IsNew = true;
        IsModified = false;
    }
}

/// <summary>Interface for ViewModels that implement CRUD operations.</summary>
public interface IViewModelCrud<TModel, TIdType>
    where TModel : class, new()
    where TIdType : struct
{
    /// <summary>The model being edited.</summary>
    TModel Model { get; }
    
    /// <summary>The ID of the model.</summary>
    TIdType ModelId { get; }
    
    /// <summary>Indicates if the model is new (create) or existing (update).</summary>
    bool IsNew { get; }
    
    /// <summary>Indicates if the model is currently being loaded.</summary>
    bool IsLoading { get; }
    
    /// <summary>Indicates if the model is currently being saved.</summary>
    bool IsSaving { get; }
    
    /// <summary>Indicates if the model has been modified.</summary>
    bool IsModified { get; }
    
    /// <summary>Indicates if the last operation was successful.</summary>
    bool OperationSuccess { get; }

    /// <summary>Saves the model.</summary>
    Task<bool> SaveAsync();
    
    /// <summary>Command to save the model.</summary>
    IAsyncRelayCommand SaveCommand { get; }
    
    /// <summary>Command to validate the model.</summary>
    IAsyncRelayCommand ValidateCommand { get; }
    
    /// <summary>Gets the model by ID.</summary>
    Task<TModel> GetByIdAsync(TIdType id, CancellationToken cancellationToken = default);
    
    /// <summary>Loads the model by ID.</summary>
    Task LoadByIdAsync(TIdType id, CancellationToken cancellationToken = default);
    
    /// <summary>Initializes the ViewModel with a specific entity ID.</summary>
    Task InitializeWithIdAsync(TIdType id, CancellationToken cancellationToken = default);
}
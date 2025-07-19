using CommunityToolkit.Mvvm.ComponentModel;
using AppWeb.ViewModels.Core.Validation;
using AppWeb.Shared.Services.Contracts;

namespace AppWeb.ViewModels.Core.Base;

/// <summary>
/// Base class for view models that require validation.
/// Extends ViewModelBase with validation capabilities.
/// </summary>
public abstract partial class ValidatableViewModel : ViewModelBase, IValidatable
{
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isValid;

    protected readonly IMessageService? _messageService;

    protected ValidatableViewModel(IMessageService? messageService = null)
    { _messageService = messageService; } //initialize the message service

    /// <summary>Gets all validation errors from the current model.</summary>
    public virtual List<string> GetAllErrors()
    {
        var results = ValidationHelper.ValidateObject(this);
        return results.Select(r => r.ErrorMessage ?? "Unknown error").ToList();
    }

    /// <summary>Validates a specific property and returns any error message.</summary>
    public virtual string? ValidateProperty(string propertyName, object? value)
    {
        var results = ValidationHelper.ValidateProperty(this, propertyName, value);
        return results.FirstOrDefault()?.ErrorMessage;
    }

    /// <summary>Shows validation errors as a formatted message.</summary>
    protected virtual async Task ShowMessageErrorValidationAsync()
    {
        if (_messageService == null) return;
        var errors = GetAllErrors(); //get all validation errors
        if (errors.Count > 0) await _messageService.ShowErrorAsync(string.Join("\n", errors));
    }

    /// <summary>Validates the current model.</summary>
    public virtual async Task<bool> ValidateAsync()
    {
        var validationResults = await ValidationHelper.ValidateObjectAsync(this);
        if (validationResults.Count > 0) { IsValid = false; ErrorMessage = validationResults.First().ErrorMessage ?? "Validation failed"; return false; }
        IsValid = true; ErrorMessage = string.Empty; return true; //return true if validation is successful
    }
}

#region Interfaces ------------------------------------------------------------
/// <summary>Interface for view models that support validation.</summary>
public interface IValidatable
{
    /// <summary>Indicates if the model is currently valid.</summary>
    bool IsValid { get; }

    /// <summary>The current error message if validation fails.</summary>
    string ErrorMessage { get; }

    /// <summary>Validates the current model.</summary>
    Task<bool> ValidateAsync();

    /// <summary>Gets all validation errors from the current model.</summary>
    List<string> GetAllErrors();

    /// <summary>Validates a specific property and returns any error message.</summary>
    string? ValidateProperty(string propertyName, object? value);
}
#endregion ---------------------------------------------------------------------
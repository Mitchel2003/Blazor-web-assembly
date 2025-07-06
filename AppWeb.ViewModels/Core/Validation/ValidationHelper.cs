using System.ComponentModel.DataAnnotations;

namespace AppWeb.ViewModels.Core.Validation;

/// <summary>Provides validation functionality for ViewModels without direct dependency on validation frameworks.</summary>
public class ValidationHelper
{
    /// <summary>Checks if the object is valid asynchronously.</summary>
    /// <param name="model">The object to validate, if null, returns false.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if the object is valid, false otherwise.</returns>
    public static Task<bool> IsValidAsync(object model)
    { return Task.FromResult(IsValid(model)); }

    /// <summary>Validates an object asynchronously using data annotations.</summary>
    /// <param name="obj">The object to validate.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of validation results.</returns>
    public static Task<List<ValidationResult>> ValidateObjectAsync(object obj)
    { return Task.FromResult(ValidateObject(obj)); }

    /// <summary>Validates an object asynchronously and returns validation errors.</summary>
    /// <param name="model">The object to validate, if null, returns an empty dictionary.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the validation errors.</returns>
    public static Task<Dictionary<string, string[]>> ValidateAsync(object model)
    { return Task.FromResult(Validate(model)); }

    /// <summary>Gets a flattened list of all validation errors.</summary>
    /// <param name="validationErrors">The dictionary of validation errors.</param>
    /// <returns>A list of all validation errors.</returns>
    public static List<string> GetAllErrors(Dictionary<string, string[]> validationErrors)
    { return validationErrors.SelectMany(kvp => kvp.Value.Select(error => $"{kvp.Key}: {error}")).ToList(); }

    /// <summary>Checks if the object is valid.</summary>
    /// <param name="model">The object to validate, if null, returns false.</param>
    /// <returns>True if the object is valid, false otherwise.</returns>
    public static bool IsValid(object model)
    {
        var validationResults = new List<ValidationResult>();
        return Validator.TryValidateObject(model, new ValidationContext(model), validationResults, validateAllProperties: true);
    }

    /// <summary>Validates an object using data annotations.</summary>
    /// <param name="obj">The object to validate.</param>
    /// <returns>A list of validation results.</returns>
    public static List<ValidationResult> ValidateObject(object obj)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(obj);
        Validator.TryValidateObject(obj, context, results, validateAllProperties: true);
        return results;
    }

    /// <summary>Validates a specific property of an object.</summary>
    /// <param name="obj">The object containing the property.</param>
    /// <param name="propertyName">The name of the property to validate.</param>
    /// <param name="value">The value to validate.</param>
    /// <returns>A list of validation results.</returns>
    public static List<ValidationResult> ValidateProperty(object obj, string propertyName, object? value)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(obj) { MemberName = propertyName };
        Validator.TryValidateProperty(value, context, results);
        return results;
    }

    /// <summary>Validates an object using DataAnnotations and returns validation errors.</summary>
    /// <param name="model">The object to validate, if null, returns an empty dictionary.</param>
    /// <returns>A dictionary of validation errors where the key is the property name and the value is an array of error messages.</returns>
    public static Dictionary<string, string[]> Validate(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
        return validationResults.GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty) //group by property name
            .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage).Where(m => m is not null).Select(m => m!).ToArray());
    }
}
using System.Globalization;

namespace AppWeb.Maui.Converters;

/// <summary>Converter that inverts a boolean value</summary>
public class InvertedBoolConverter : IValueConverter
{
    /// <summary>Converts a boolean value to its inverse</summary>
    /// <param name="value">The boolean value to invert</param>
    /// <param name="targetType">Type that the value is being converted to</param>
    /// <param name="parameter">Optional parameter (not used in this converter)</param>
    /// <param name="culture">Culture information (not used in this converter)</param>
    /// <returns>The inverted boolean value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        { return !boolValue; }
        return true;
    }

    /// <summary>Converts back from an inverted boolean to the original value</summary>
    /// <param name="value">The boolean value to invert</param>
    /// <param name="targetType">Type that the value is being converted to</param>
    /// <param name="parameter">Optional parameter (not used in this converter)</param>
    /// <param name="culture">Culture information (not used in this converter)</param>
    /// <returns>The inverted boolean value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    { return Convert(value, targetType, parameter, culture); }
}
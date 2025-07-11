using System.Globalization;

namespace AppWeb.Maui.Converters;

/// <summary>Converts a boolean value to a password visibility indicator text</summary>
public class BoolToPasswordVisibilityConverter : IValueConverter
{
    /// <summary>Converts a boolean to a "Show" or "Hide" text for password visibility</summary>
    /// <param name="value">Boolean value indicating if the password is currently visible</param>
    /// <param name="targetType">Type that the value is being converted to</param>
    /// <param name="parameter">Optional parameter (not used in this converter)</param>
    /// <param name="culture">Culture information (not used in this converter)</param>
    /// <returns>"Hide" if the value is true, otherwise "Show"</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isVisible) return isVisible ? "Hide" : "Show";
        return "Show";
    }

    /// <summary>Converts from a string back to a boolean value (not implemented)</summary>
    /// <exception cref="NotImplementedException">This method is not implemented</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    { throw new NotImplementedException(); }
}
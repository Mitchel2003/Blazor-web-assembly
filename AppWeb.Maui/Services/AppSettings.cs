using Microsoft.Extensions.Configuration;

namespace AppWeb.Maui.Services;

/// <summary>Service for accessing application configuration settings.</summary>
public class AppSettings
{
    private readonly IConfiguration _configuration;

    public AppSettings(IConfiguration configuration)
    { _configuration = configuration; }

    /// <summary>Gets the base URL of the API.</summary>
    public string ApiBaseUrl => _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001";

    /// <summary>Gets the API timeout in seconds.</summary>
    public int ApiTimeout => int.TryParse(_configuration["ApiSettings:Timeout"], out int timeout) ? timeout : 30;

    /// <summary>Gets the application name.</summary>
    public string AppName => _configuration["AppSettings:AppName"] ?? "AppWeb MAUI";

    /// <summary>Gets the application version.</summary>
    public string Version => _configuration["AppSettings:Version"] ?? "1.0.0";

    /// <summary>Gets the current environment.</summary>
    public string Environment => _configuration["AppSettings:Environment"] ?? "Development";
}
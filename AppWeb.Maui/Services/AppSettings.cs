using Microsoft.Extensions.Configuration;

namespace AppWeb.Maui.Services;

/// <summary>MAUI application settings</summary>
public class AppSettings
{
    /// <summary>API base URL</summary>
    public string ApiBaseUrl { get; set; } = "https://localhost:7240";

    /// <summary>Default constructor</summary>
    public AppSettings() { }

    /// <summary>Constructor with configuration</summary>
    public AppSettings(IConfiguration configuration)
    { configuration.Bind(this); }
}
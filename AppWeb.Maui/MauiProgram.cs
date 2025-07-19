using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AppWeb.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    { //Define the compilation variable to disable the Debugger.Break() behavior
        Environment.SetEnvironmentVariable("DISABLE_XAML_GENERATED_BREAK_ON_UNHANDLED_EXCEPTION", "true");

        var builder = MauiApp.CreateBuilder();
        
        //Configure application with fonts and services
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).AddServices(); //Register all application services via extension method

        //Configure debug logging
#if DEBUG  
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        return app;
    }
}
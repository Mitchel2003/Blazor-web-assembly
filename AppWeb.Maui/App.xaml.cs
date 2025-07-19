using System.Diagnostics;

namespace AppWeb.Maui;

public partial class App : Application
{
    public App()
    {
        try
        {
            // Inicializar componentes de la aplicación
            InitializeComponent();
            
            // Establecer la página principal
            MainPage = new AppShell();
            
            // Configurar manejo de excepciones no controladas
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error al inicializar la aplicación: {ex.Message}");
            Debug.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }
    
    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        Debug.WriteLine($"Excepción no controlada: {exception?.Message}");
        Debug.WriteLine($"StackTrace: {exception?.StackTrace}");
    }
    
    private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        Debug.WriteLine($"Excepción de tarea no observada: {e.Exception.Message}");
        Debug.WriteLine($"StackTrace: {e.Exception.StackTrace}");
        e.SetObserved(); // Marcar como observada para evitar que la aplicación se cierre
    }
}
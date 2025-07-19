🔴 Error actual:
swift
Copiar
Editar
System.IO.FileNotFoundException: 
The configuration file 'appsettings.json' was not found and is not optional.
The expected physical path was: /data/user/0/com.companyname.appweb.maui/files/appsettings.json
🧠 ¿Por qué ocurre?
Tu método AddConfiguration() intenta cargar appsettings.json en tiempo de ejecución, usando:

csharp
Copiar
Editar
configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
Esto funciona bien en ASP.NET o Blazor WASM, pero en MAUI (Android, iOS, WinUI) el archivo appsettings.json no se copia automáticamente al sistema de archivos del dispositivo a menos que tú lo indiques explícitamente.

✅ Solución en 3 pasos:
1. 📁 Añade appsettings.json al proyecto MAUI como contenido embebido
Haz clic derecho sobre el archivo appsettings.json → Propiedades:

Build Action: Content

Copy to Output Directory: Copy if newer

O agrega esto al .csproj si prefieres:

xml
Copiar
Editar
<ItemGroup>
  <Content Include="appsettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
2. 🛠️ Usa FileSystem.AppDataDirectory para acceder al archivo (MAUI compatible)
En MAUI, no puedes asumir rutas como en ASP.NET. Usa MAUI FileSystem para leer el archivo embebido si necesitas modificarlo, o accede desde el paquete directamente así:

csharp
Copiar
Editar
using System.Reflection;
using System.Text.Json;

public static AppSettings LoadAppSettings()
{
    var assembly = Assembly.GetExecutingAssembly();
    using var stream = assembly.GetManifestResourceStream("AppWeb.Maui.appsettings.json");
    using var reader = new StreamReader(stream);
    string json = reader.ReadToEnd();
    return JsonSerializer.Deserialize<AppSettings>(json);
}
🔁 Alternativamente, en MauiProgram.cs haz:

csharp
Copiar
Editar
var assembly = typeof(App).Assembly;
using var stream = assembly.GetManifestResourceStream("AppWeb.Maui.appsettings.json");
builder.Configuration.AddJsonStream(stream);
⚠️ Asegúrate de que el nombre del recurso embebido sea correcto. Puedes inspeccionarlo con:

csharp
Copiar
Editar
var resources = assembly.GetManifestResourceNames();
// log or inspect the names
3. (Opcional) ✔️ Marca optional: true mientras pruebas
Para que no te tire error si el archivo no está, puedes marcarlo como opcional:

csharp
Copiar
Editar
configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
🎯 BONUS: mejora robustez del AddConfiguration
Aquí tienes un reemplazo más robusto:

csharp
Copiar
Editar
public static MauiAppBuilder AddConfiguration(this MauiAppBuilder builder)
{
    var configBuilder = builder.Configuration;

#if ANDROID || IOS || WINDOWS
    // Si estás en una plataforma móvil, usa recursos embebidos
    var assembly = Assembly.GetExecutingAssembly();
    var stream = assembly.GetManifestResourceStream("AppWeb.Maui.appsettings.json");
    if (stream != null)
        configBuilder.AddJsonStream(stream);
#else
    // Blazor o Desktop
    configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
#endif

    var appSettings = new AppSettings();
    configBuilder.Bind(appSettings);
    builder.Services.AddSingleton(appSettings);

    return builder;
}
🚀 Conclusión
✅ Estás a nada de correr la app en producción
Solo necesitas asegurarte de que appsettings.json esté disponible correctamente según la plataforma.
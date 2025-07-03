# AppWeb.ViewModels

## Arquitectura de ViewModels Compartidos

Este proyecto implementa una arquitectura de ViewModels compartidos que permite la reutilización de lógica de presentación entre diferentes plataformas como Blazor WebAssembly y .NET MAUI.

### Estructura

```
AppWeb.ViewModels/
├── Core/              # Abstracciones compartidas, base y utilidades
│   ├── Base/          # Clases base de ViewModel y abstracciones
│   ├── Services/      # Interfaces de servicios que necesitan los ViewModels
│   └── Extensions/    # Extensiones de utilidad
├── Features/          # ViewModels organizados por funcionalidad
│   ├── Users/         # Subdividido por caso de uso específico
│   │   ├── CreateUserVM.cs   # ViewModel para crear usuarios
│   │   └── UpdateUserVM.cs   # ViewModel para actualizar usuarios
│   └── Auth/          # Funcionalidades de autenticación
└── Plataform/         # Implementaciones específicas de plataforma (opcional)
    ├── Blazor/        # Adaptadores específicos para Blazor
    └── Maui/          # Adaptadores específicos para MAUI
```

### Características principales

- **Separación por casos de uso**: Cada ViewModel tiene una única responsabilidad clara (SRP)
- **Independencia de la UI**: Los ViewModels no tienen referencias directas a componentes UI
- **Abstracción de servicios**: Interfaces que permiten implementaciones específicas por plataforma
- **Inmutabilidad**: Uso de propiedades con getter privados para promover un flujo unidireccional de datos

### Patrones utilizados

- **MVVM (Model-View-ViewModel)**: Patrón fundamental para separar la UI de la lógica de presentación
- **Dependency Injection**: Todas las dependencias se inyectan para facilitar el testing y la escalabilidad
- **Repository Pattern**: Los servicios abstraen el acceso a datos
- **Command Pattern**: Para encapsular acciones iniciadas por el usuario

### Uso básico

#### 1. Registro de ViewModels en el contenedor DI

```csharp
public static IServiceCollection AddViewModels(this IServiceCollection services)
{
    // Registrar ViewModels de Usuarios
    services.AddTransient<ICreateUserVM, CreateUserVM>();
    services.AddTransient<IUpdateUserVM, UpdateUserVM>();
    
    // Otros ViewModels...
    return services;
}
```

#### 2. Registro de implementaciones específicas de plataforma

```csharp
// En Blazor
services.AddScoped<IUsersService, BlazorUsersService>();
services.AddScoped<INavigationService, BlazorNavigationService>();
services.AddScoped<IMessageService, BlazorMessageService>();

// En MAUI (ejemplo)
services.AddSingleton<IUsersService, MauiUsersService>();
services.AddSingleton<INavigationService, MauiNavigationService>();
services.AddSingleton<IMessageService, MauiMessageService>();
```

#### 3. Uso en componentes Blazor

```csharp
// En un componente Razor
@inject ICreateUserVM ViewModel

<FormUser Input="@ViewModel.Input" 
          Loading="@ViewModel.IsSaving"
          Success="@ViewModel.SaveSuccess"
          OnSubmit="@(() => ViewModel.CreateUserAsync())" />
```

#### 4. Uso en MAUI (ejemplo futuro)

```csharp
// En un ContentPage de MAUI
public MyPage()
{
    InitializeComponent();
    BindingContext = ServiceProvider.GetService<ICreateUserVM>();
}
```

### Consideraciones de implementación

- Los ViewModels son transient para mantener un estado limpio en cada navegación
- Cada ViewModel sólo expone las propiedades y métodos necesarios para su caso de uso específico
- Las propiedades observables se generan con `[ObservableProperty]` de CommunityToolkit.Mvvm 
# AppWeb.ViewModels

Este proyecto define los ViewModels utilizados en la aplicación, siguiendo el patrón MVVM (Model-View-ViewModel). Los ViewModels implementados aquí son independientes de la plataforma y pueden ser utilizados tanto en Blazor como en MAUI.

## Arquitectura

Los ViewModels siguen una estructura jerárquica de herencia:

1. **ViewModelBase**: Clase base para todos los ViewModels, proporciona funcionalidad común como propiedades de título, inicialización y navegación.
2. **ValidatableViewModel**: Extiende ViewModelBase con capacidades de validación, manejo de errores y notificación de errores.
3. **ViewModelCrud**: Extiende ValidatableViewModel para proporcionar operaciones CRUD estándar (Create, Read, Update, Delete).
4. **ViewModels concretos**: Implementaciones específicas para cada funcionalidad, como CreateUserVM, UpdateUserVM, LoginVM, etc.

Esta jerarquía garantiza:
- Reutilización de código
- Consistencia en la implementación
- Separación clara de responsabilidades

## Principios de Diseño

1. Mantener la independencia de la plataforma
2. Seguir el principio de responsabilidad única (SRP)
3. Proporcionar una API clara y coherente
4. Usar validación para todas las entradas de usuario
5. Manejar todas las excepciones y proporcionar retroalimentación al usuario

## Key Features

- **Complete Platform Independence**: All ViewModels are agnostic of their UI implementation
- **Domain Object Factory Pattern**: No direct instantiation of domain objects, ensuring loose coupling
- **Enhanced Command Pattern**: First-class support for commands with automatic busy state handling
- **Built-in Validation**: Platform-independent validation system that works across all targets
- **Advanced State Management**: Comprehensive state tracking for all operations
- **Clean Dependency Injection**: All dependencies are explicitly declared and injected

## Architecture Overview

```
AppWeb.ViewModels/
├── Core/                   # Core abstractions and base classes
│   ├── Base/               # ViewModel base classes with core functionality
│   │   ├── ViewModelBase.cs         # Base for all ViewModels
│   │   ├── ViewModelCrud.cs         # Base for CRUD ViewModels
│   │   ├── ValidatableViewModel.cs  # Base for ViewModels with validation
│   ├── Factory/            # Factory pattern for domain objects
│   │   ├── IModelFactory.cs         # Factory interface
│   │   └── DefaultModelFactory.cs   # Default implementation
│   ├── Services/           # Business service interfaces
│   │   ├── IAuthService.cs          # Authentication service
│   │   ├── IUsersService.cs         # User management service
│   │   ├── IMessageService.cs       # UI notification abstraction
│   │   └── INavigationService.cs    # Navigation abstraction
│   └── Validation/         # Validation abstractions
│       └── ValidationHelper.cs      # Validation utilities
└── Features/               # Feature-specific ViewModels
    ├── Auth/               # Authentication ViewModels
    │   └── LoginVM.cs              # Login functionality
    └── Users/              # User management ViewModels
        ├── CreateUserVM.cs         # User creation
        └── UpdateUserVM.cs         # User editing
```

## Usage Guide

### 1. Registration

Register ViewModels and services in your application's DependencyInjection:

```csharp
// Register base services
services.AddViewModelsCore();

// Register all ViewModels from assembly
services.AddViewModelsFromAssembly(typeof(LoginVM).Assembly);

// Register platform-specific implementations
services.AddScoped<INavigationService, BlazorNavigationService>();
services.AddScoped<IMessageService, BlazorMessageService>();
services.AddScoped<IAuthService, BlazorAuthService>();
services.AddScoped<IUsersService, BlazorUsersService>();
```

### 2. Usage in Blazor Components

```razor
@inject ILoginVM ViewModel

<EditForm Model="@ViewModel.Model" OnValidSubmit="@(() => ViewModel.LoginCommand.ExecuteAsync(context))">
    <FluentValidator TModel="LoginInput" />
    
    <MudTextField @bind-Value="ViewModel.Model.Email" Label="Email" />
    <MudTextField @bind-Value="ViewModel.Model.Password" 
                 InputType="@(ViewModel.ShowPassword ? InputType.Text : InputType.Password)" 
                 Label="Password" />
                 
    <MudButton Disabled="@ViewModel.IsLoading"
              Variant="Variant.Filled"
              Color="Color.Primary"
              ButtonType="ButtonType.Submit">
        @if(ViewModel.IsLoading)
        {
            <MudProgressCircular Size="Size.Small" Indeterminate="true" />
            <span>Processing...</span>
        }
        else
        {
            <span>Login</span>
        }
    </MudButton>
</EditForm>

@code {
    protected override async Task OnInitializedAsync()
    {
        await ViewModel.InitializeAsync();
    }
}
```

### 3. Usage in MAUI (Future-proof)

```csharp
public MainPage()
{
    InitializeComponent();
    BindingContext = ServiceProvider.GetService<ILoginVM>();
}

protected override async void OnAppearing()
{
    base.OnAppearing();
    await ((ILoginVM)BindingContext).InitializeAsync();
}
```

## Key Benefits

1. **Testability**: All ViewModels can be unit tested without UI dependencies
2. **Code Reuse**: No duplication between platforms
3. **Separation of Concerns**: UI logic is isolated from business logic
4. **Type Safety**: All bindings and operations are strongly typed
5. **Clean Architecture**: Follows SOLID principles and clean architecture patterns

## ViewModel Lifecycle

1. **Construction**: Dependencies injected via constructor
2. **Initialization**: `InitializeAsync()` loads initial data
3. **User Interaction**: Commands handle user actions
4. **Disposal**: `Dispose()` releases resources

## Best Practices

1. Always use commands for user actions
2. Never directly instantiate domain objects in ViewModels
3. Keep ViewModels focused on a single responsibility
4. Use validation for all user inputs
5. Handle all exceptions and provide user feedback

## Sistema de Navegación

El sistema de navegación utiliza un enfoque unificado basado en configuración para simplificar y estandarizar la navegación entre vistas:

### Clase NavigationConfig

Esta clase proporciona una forma flexible y fluida de configurar la navegación:

```csharp
// Ejemplo de uso del sistema de navegación
await _navigationService.NavigateToAsync(
    new NavigationConfig(NavigationConfig.Routes.Users)
        .WithParam("id", userId)
        .WithForceLoad(true)
);
```

### Rutas Predefinidas

Las rutas comunes de la aplicación están definidas como constantes en `NavigationConfig.Routes`:

- `Home`: Página principal
- `Login`: Página de inicio de sesión
- `Logout`: Página de cierre de sesión
- `Users`: Lista de usuarios
- `CreateUser`: Creación de usuario
- `EditUser`: Edición de usuario

Este enfoque unificado evita la duplicación de código y proporciona un punto central para gestionar la navegación en la aplicación.

## Servicios Principales

Los ViewModels dependen de interfaces de servicios que deben ser implementadas por la plataforma específica (Blazor, MAUI):

- **INavigationService**: Manejo de la navegación entre vistas
- **IMessageService**: Muestra mensajes y diálogos al usuario
- **IModelFactory**: Creación de modelos y mapeo entre tipos
- **IAuthService**: Servicios de autenticación
- **IUsersService**: Servicios para operaciones CRUD de usuarios

## Registro de Servicios

Los ViewModels y servicios se registran en el contenedor de dependencias a través del método `AddViewModels()` en la clase `DependencyInjection`, que debe ser llamado al inicio de la aplicación:

```csharp
// En Program.cs (Blazor) o MauiProgram.cs (MAUI)
builder.Services.AddViewModels();
```

## Eventos

Los ViewModels utilizan eventos para notificar a las vistas sobre cambios importantes:

- `UserCreated`: Notifica cuando un usuario ha sido creado exitosamente
- `UserUpdated`: Notifica cuando un usuario ha sido actualizado exitosamente
- `LoginSuccessful`: Notifica cuando un inicio de sesión ha sido exitoso
- `LogoutCompleted`: Notifica cuando se ha completado el cierre de sesión

## Validación

La validación se maneja a través de la clase `ValidatableViewModel`, que proporciona:

- Validación basada en atributos de modelo
- Soporte para validación asíncrona
- Métodos para obtener y mostrar errores de validación
- Integración con `IMessageService` para mostrar errores al usuario

## Ciclo de Vida

1. Los ViewModels se crean a través del contenedor de dependencias
2. Se llama a `InitializeAsync()` para inicializar el ViewModel
3. Para operaciones CRUD, se utilizan los métodos específicos como `LoadByIdAsync()`, `SaveAsync()`, etc.
4. Los ViewModels notifican a las vistas a través de eventos cuando ocurren cambios importantes 
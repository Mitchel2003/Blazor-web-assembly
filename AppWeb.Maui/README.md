# AppWeb MAUI

This is the MAUI client application for the AppWeb project, implementing a professional cross-platform mobile and desktop application using the MVVM (Model-View-ViewModel) pattern.

## Architecture Overview

The MAUI project follows the same architectural principles as the rest of the AppWeb solution, maintaining clean separation of concerns while providing a native mobile/desktop experience. The project leverages the existing ViewModels library, allowing for code reuse between the Blazor WebAssembly and MAUI applications.

### Project Structure

- **Models**: Domain-specific models used by the MAUI application
- **Views**: MAUI XAML UI components organized by feature
- **Services**: MAUI-specific implementations of interfaces defined in the ViewModels library
- **Behaviors**: Custom behaviors to enhance XAML capabilities
- **Controls**: Custom controls for reusability
- **Converters**: Value converters for XAML bindings

### Key Architecture Patterns

- **MVVM**: The application uses the Model-View-ViewModel pattern, with ViewModels shared with the Blazor application
- **Dependency Injection**: All services are registered and resolved through the DI container
- **Repository Pattern**: Data access is abstracted through repository interfaces
- **Command Pattern**: User actions are encapsulated in commands

## Features

- User authentication and authorization
- User management (CRUD operations)
- Secure token storage and handling
- Responsive UI for all device sizes
- Configuration through appsettings.json

## Getting Started

1. Ensure you have the .NET 7.0 SDK installed
2. Install the .NET MAUI workload: `dotnet workload install maui`
3. Open the AppWeb solution in Visual Studio
4. Set AppWeb.MAUI as the startup project
5. Update the API URL in `appsettings.json` to point to your backend API
6. Build and run the application

## Development Guidelines

- Always use XAML for UI definitions when possible
- Follow the MVVM pattern strictly - no code-behind logic beyond the absolute minimum
- Leverage existing ViewModels without modification
- Use DI for all services
- Create thorough unit tests for all services
- Follow the project's established naming conventions

## Platform-Specific Considerations

For platform-specific functionality, use the following approach:

```csharp
// Example of platform-specific code
if (DeviceInfo.Platform == DevicePlatform.Android)
{
    // Android-specific code
}
else if (DeviceInfo.Platform == DevicePlatform.iOS)
{
    // iOS-specific code
}
else
{
    // Default or other platform code
}
``` 
# AppWeb Architecture: Clean Separation of Concerns

## Overview

AppWeb implements a sophisticated architecture designed to support both Blazor WebAssembly and MAUI clients while maintaining strict separation of concerns. This document outlines the design principles, layer responsibilities, and implementation patterns that ensure a clean, maintainable codebase.

## Architecture Layers

### Core Domain
- **AppWeb.Domain**: Contains domain entities, business rules, and domain interfaces
- **AppWeb.Shared**: Cross-cutting DTOs, contracts, and shared types used across all layers
- **AppWeb.Application**: Application services, use cases, and business logic implementation

### Infrastructure
- **AppWeb.Infrastructure**: Data access, external services integration, and implementation of domain interfaces

### Client Logic
- **AppWeb.ViewModels**: Platform-independent ViewModels implementing the MVVM pattern
- **AppWeb.SharedClient**: Shared client services that adapt backend contracts for UI consumption

### Platform UI
- **AppWeb.Client**: Blazor WebAssembly UI components and platform-specific implementations
- **AppWeb.MAUI** (Future): MAUI UI components and platform-specific implementations

### Server Host
- **AppWeb.Server**: API endpoints and hosting infrastructure for WebAssembly client

## Key Architectural Decisions

### 1. Strict UI Component Isolation

UI component libraries (like MudBlazor) are strictly isolated to the UI layer:
- **Correct**: MudBlazor references in AppWeb.Client and AppWeb.SharedClient
- **Incorrect**: MudBlazor references in AppWeb.Server or AppWeb.Application

### 2. WebAssembly Rendering Strategy

The application uses an optimized WebAssembly rendering strategy:
- Server components don't use UI libraries directly
- Client components handle all UI rendering through WebAssembly
- `InteractiveWebAssemblyRenderMode(prerender: false)` ensures clean separation

### 3. View-ViewModel Separation

The MVVM pattern is implemented across platform boundaries:
- ViewModels are platform-agnostic (no UI framework dependencies)
- Views implement platform-specific rendering (Blazor, MAUI)
- SharedClient adapters connect ViewModels to platform capabilities

## Dependency Flow

Dependencies flow inward toward the core domain:
1. UI layers depend on SharedClient and ViewModels
2. SharedClient depends on Shared contracts
3. Server depends on Application and Infrastructure
4. Infrastructure depends on Application
5. Application depends on Domain and Shared

## UI Component Guidelines

### For Blazor WebAssembly Components:
1. Reference UI libraries directly in the Client project
2. Use platform-specific services registered in DependencyInjection.cs
3. Consume ViewModels through dependency injection
4. Keep rendering logic in Razor components

### For Shared Services:
1. Implement platform-agnostic interfaces from ViewModels
2. Use adapters to connect to platform capabilities
3. Avoid direct UI framework dependencies when possible

### For Server Components:
1. Focus on hosting and API functionality
2. Avoid direct references to UI libraries
3. Use minimal components necessary for hosting the WebAssembly client
4. Delegate UI rendering to the client through proper render modes

## Conclusion

This architecture enables maximum code reuse while maintaining clean separation between server and client concerns. It supports both current Blazor WebAssembly and future MAUI implementations without compromising architectural integrity.

The separation of UI components from server code ensures that the backend can focus on its core responsibilities while allowing the frontend to independently leverage modern UI frameworks. 
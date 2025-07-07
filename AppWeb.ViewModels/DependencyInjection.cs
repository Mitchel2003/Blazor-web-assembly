using Microsoft.Extensions.DependencyInjection;
using AppWeb.ViewModels.Features.Users;
using AppWeb.ViewModels.Features.Auth;
using AppWeb.ViewModels.Core.Factory;
using System.Reflection;

namespace AppWeb.ViewModels;

/// <summary>Configures dependency injection for ViewModels.</summary>
public static class DependencyInjection
{
    /// <summary>Adds all ViewModels to the service collection.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection with ViewModels registered.</returns>
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddViewModelsFromAssembly(Assembly.GetExecutingAssembly());
        
        //Register auth and user ViewModels explicitly
        services.AddScoped<ILoginVM, LoginVM>();
        services.AddScoped<ILogoutVM, LogoutVM>();
        services.AddScoped<ICreateUserVM, CreateUserVM>();
        services.AddScoped<IUpdateUserVM, UpdateUserVM>();
        services.AddScoped<ITableUserVM, TableUserVM>();
        services.AddScoped<ITableUsersPageVM, TableUsersPageVM>();
        return services;
    }
    
    /// <summary>Adds all ViewModels from the specified assembly to the specified <see cref="IServiceCollection"/>.</summary>
    public static IServiceCollection AddViewModelsFromAssembly(this IServiceCollection services, Assembly assembly)
    { //Register core services, important for platform-agnostic ViewModels
        services.AddViewModelsCore();

        //Find all interfaces that represent ViewModels
        var viewModelInterfaces = assembly.GetTypes().Where(t => t.IsInterface && t.Name.EndsWith("VM"))
            .ToDictionary(i => i, i => assembly.GetTypes().FirstOrDefault(t => !t.IsInterface && !t.IsAbstract && i.IsAssignableFrom(t)));

        //Register the ViewModel with the interface, ensuring it is scoped
        foreach (var (vmInterface, vmImplementation) in viewModelInterfaces)
        { if (vmImplementation != null) services.AddScoped(vmInterface, vmImplementation); }
        return services;
    }
    
    /// <summary>Adds all services needed for platform-agnostic ViewModels.</summary>
    public static IServiceCollection AddViewModelsCore(this IServiceCollection services)
    { //Get the singleton instance of the ModelFactory
        var factory = DefaultModelFactory.Instance;
        ModelFactoryProvider.Initialize(factory);
        services.AddSingleton<IModelFactory>(_ => factory);
        return services; //Register in the DI container and return collection
    }
}
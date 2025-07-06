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
    { //Register ModelFactory as Singleton since it is stateless and thread-safe
        RegisterModelFactory(services);

        //Register ViewModels as Transient services, so that each
        //component receives a new instance and avoids shared state problems
        services.AddTransient<ILoginVM, LoginVM>();
        services.AddTransient<ILogoutVM, LogoutVM>();
        services.AddTransient<ICreateUserVM, CreateUserVM>();
        services.AddTransient<IUpdateUserVM, UpdateUserVM>();
        return services;
    }
    
    /// <summary>Adds all ViewModels from the specified assembly to the specified <see cref="IServiceCollection"/>.</summary>
    public static IServiceCollection AddViewModelsFromAssembly(this IServiceCollection services, Assembly assembly)
    { //Register core services, important for platform-agnostic ViewModels
        RegisterModelFactory(services);
        
        //Find all interfaces that represent ViewModels
        var viewModelInterfaces = assembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("VM"))
            .ToDictionary(i => i, i => assembly.GetTypes().FirstOrDefault(t => 
                !t.IsInterface && !t.IsAbstract && i.IsAssignableFrom(t)));
        
        //Register all ViewModel implementations as Transient
        foreach (var (vmInterface, vmImplementation) in viewModelInterfaces)
        { if (vmImplementation != null) { services.AddTransient(vmInterface, vmImplementation); } }
        return services;
    }
    
    /// <summary>Adds all services needed for platform-agnostic ViewModels.</summary>
    public static IServiceCollection AddViewModelsCore(this IServiceCollection services)
    { //Register only core services, important for platform-agnostic ViewModels
        RegisterModelFactory(services);
        return services;
    }

    #region Helpers ------------------------------------------------------------
    /// <summary>Registers the IModelFactory as a Singleton using the DefaultModelFactory instance.</summary>
    private static void RegisterModelFactory(IServiceCollection services)
    { //Get the singleton instance of the ModelFactory
        var factory = DefaultModelFactory.Instance;
        ModelFactoryProvider.Initialize(factory); //Initialize the static provider
        services.AddSingleton<IModelFactory>(_ => factory); //Register in the DI container
    }
    #endregion ---------------------------------------------------------------------
}
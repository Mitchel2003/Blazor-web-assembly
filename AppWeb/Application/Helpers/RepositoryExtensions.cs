using System.Reflection;

namespace AppWeb.Application.Helpers;

/// <summary>
/// IServiceCollection extension to automatically register repository interfaces and their
/// concrete implementations following the <c>I{Entity}Repository</c> ↔ <c>{Entity}Repository</c> convention.
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Scans the specified assemblies and registers each interface ending with <c>Repository</c>
    /// to its concrete implementation as a scoped service.
    /// </summary>
    /// <param name="services">The DI container.</param>
    /// <param name="contractsAssembly">Assembly that contains repository interfaces (e.g., Application.Contracts.Persistence).</param>
    /// <param name="implementationsAssembly">Assembly that contains repository implementations (e.g., Infrastructure.Persistence.Repositories).</param>
    /// <returns>The same IServiceCollection for chaining.</returns>
    public static IServiceCollection AddAllRepositories(this IServiceCollection services,Assembly contractsAssembly,Assembly implementationsAssembly)
    {
        var interfaceTypes = contractsAssembly.GetTypes().Where(t => t.IsInterface && t.Name.EndsWith("Repository") && !t.IsGenericType).ToList();
        foreach (var interfaceType in interfaceTypes)
        {
            var implementation = implementationsAssembly.GetTypes().FirstOrDefault(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));
            if (implementation is not null) { services.AddScoped(interfaceType, implementation); }
        }
        return services;
    }
}
using HotChocolate.Execution.Configuration;
using System.Reflection;

namespace AppWeb.Application.Shared;

public static class Helpers
{
    /**
     * Helper method to get the model type by name.
     * Let's assume the models are in Models namespace.
     * Examples: User, Product, Order; system dinamic.
     */
    public static Type GetModelType(string model) =>
        Type.GetType($"AppLogin.Models.{model}", throwOnError: false, ignoreCase: true)
            ?? throw new GraphQLException(ErrorBuilder.New().SetMessage($"Model '{model}' not found.").SetCode("MODEL_NOT_FOUND").Build());
    /*---------------------------------------------------------------------------------------------------------*/

    /*--------------------------------------------------actions--------------------------------------------------*/
    /** Helper method to add all model types from a specific assembly and namespace to the GraphQL executor builder */
    public static IRequestExecutorBuilder AddAllModelTypes(this IRequestExecutorBuilder builder, Assembly assembly, string @namespace)
    {
        var types = assembly.GetTypes().Where(t => t.IsClass && t.Namespace == @namespace).ToList();
        foreach (var type in types) builder = builder.AddType(type);
        return builder;
    }

    /** Helper method to add all mution types from a specific assembly and namespace to the GraphQL executor builder */
    public static IRequestExecutorBuilder AddAllExtensions(this IRequestExecutorBuilder builder, Assembly assembly, string @namespace)
    {
        var Types = assembly.GetTypes().Where(t => t.IsClass && t.IsPublic && !t.IsAbstract && t.Namespace == @namespace && !t.Name.StartsWith("<")).ToList();
        foreach (var type in Types) builder = builder.AddTypeExtension(type);
        return builder;
    }
}
namespace AppWeb.ViewModels.Core.Factory;

/// <summary>
/// Factory interface for creating model objects without direct instantiation in ViewModels.
/// This abstraction allows ViewModels to be completely independent from domain/DTOs implementation.
/// </summary>
public interface IModelFactory
{
    /// <summary>Creates a new instance of the specified model type.</summary>
    /// <typeparam name="TModel">The model type to create.</typeparam>
    /// <returns>A new instance of the specified model type.</returns>
    TModel Create<TModel>() where TModel : class, new();

    /// <summary>Creates a new instance of the specified model type with initialization.</summary>
    /// <typeparam name="TModel">The model type to create.</typeparam>
    /// <param name="initializer">Action to initialize the model.</param>
    /// <returns>A new instance of the specified model type.</returns>
    TModel Create<TModel>(Action<TModel> initializer) where TModel : class, new();

    /// <summary>Creates a new instance by copying properties from another object.</summary>
    /// <typeparam name="TModel">The model type to create.</typeparam>
    /// <typeparam name="TSource">The source object type.</typeparam>
    /// <param name="source">The source object to copy from.</param>
    /// <returns>A new instance with copied properties.</returns>
    TModel CreateFrom<TModel, TSource>(TSource source)
        where TModel : class, new()
        where TSource : class;
}
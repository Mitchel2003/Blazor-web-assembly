namespace AppWeb.ViewModels.Core.Factory;

/// <summary>
/// Static factory for creating model objects without dependency injection.
/// This provides the same functionality as DefaultModelFactory but through static methods.
/// </summary>
public static class StaticModelFactory
{
    /// <summary>Creates a new instance of the specified model type.</summary>
    public static TModel Create<TModel>() where TModel : class, new()
    { return ModelFactoryProvider.Current.Create<TModel>(); }

    /// <summary>Creates a new instance of the specified model type with initialization.</summary>
    public static TModel Create<TModel>(Action<TModel> initializer) where TModel : class, new()
    { return ModelFactoryProvider.Current.Create(initializer); }

    /// <summary>Creates a new instance by copying properties from another object.</summary>
    public static TModel CreateFrom<TModel, TSource>(TSource source)
        where TModel : class, new()
        where TSource : class
    { return ModelFactoryProvider.Current.CreateFrom<TModel, TSource>(source); }
} 
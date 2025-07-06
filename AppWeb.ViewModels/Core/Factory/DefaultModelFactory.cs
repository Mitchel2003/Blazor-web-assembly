using System.Reflection;

namespace AppWeb.ViewModels.Core.Factory;

/// <summary>Default implementation of the IModelFactory interface using the Singleton pattern.</summary>
public class DefaultModelFactory : IModelFactory
{
    private static DefaultModelFactory? _instance;
    private static readonly object _lock = new object();

    /// <summary>Gets the singleton instance of the DefaultModelFactory.</summary>
    public static DefaultModelFactory Instance
    {
        get
        {
            if (_instance == null) { lock (_lock) { _instance ??= new DefaultModelFactory(); } }
            return _instance;
        }
    }

    /// <summary>Private constructor to prevent direct instantiation.</summary>
    private DefaultModelFactory() { }

    /// <summary>Creates a new instance of the specified model type.</summary>
    public TModel Create<TModel>() where TModel : class, new() => new TModel();

    /// <summary>Creates a new instance of the specified model type with initialization.</summary>
    public TModel Create<TModel>(Action<TModel> initializer) where TModel : class, new()
    {
        var model = new TModel();
        initializer?.Invoke(model);
        return model;
    }

    /// <summary>Creates a new instance by copying properties from another object.</summary>
    public TModel CreateFrom<TModel, TSource>(TSource source)
        where TModel : class, new()
        where TSource : class
    {
        if (source == null) return null!;
        var model = new TModel();
        CopyProperties(source, model);
        return model;
    }

    /// <summary>Copies matching properties from source to destination object.</summary>
    private void CopyProperties<TSource, TDestination>(TSource source, TDestination destination)
        where TSource : class
        where TDestination : class
    {
        var sourceProperties = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name); //get all properties of the destination type, and convert to dictionary

        foreach (var sourceProperty in sourceProperties)
        {
            if (!destinationProperties.TryGetValue(sourceProperty.Name, out var destinationProperty)) continue;
            if (!destinationProperty.CanWrite) continue; //check if property is writable, if not, skip
            if (!IsAssignable(sourceProperty.PropertyType, destinationProperty.PropertyType)) continue;

            //get value from source property
            var value = sourceProperty.GetValue(source);
            destinationProperty.SetValue(destination, value);
        }
    }

    /// <summary>Determines if a value of sourceType can be assigned to a property of destinationType.</summary>
    private bool IsAssignable(Type sourceType, Type destinationType)
    {
        return destinationType.IsAssignableFrom(sourceType)
            || (Nullable.GetUnderlyingType(destinationType) ?? destinationType)
            .IsAssignableFrom(Nullable.GetUnderlyingType(sourceType) ?? sourceType);
    }
}
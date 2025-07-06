namespace AppWeb.ViewModels.Core.Factory;

/// <summary>
/// Global access point to the IModelFactory instance.
/// This allows code that can't use dependency injection to still access the model factory.
/// </summary>
public static class ModelFactoryProvider
{
    private static IModelFactory? _factory;
    private static readonly object _lock = new object();

    /// <summary>Gets or sets the global IModelFactory instance.</summary>
    public static IModelFactory Current
    {
        get
        {
            if (_factory == null) { lock (_lock) { _factory ??= DefaultModelFactory.Instance; } }
            return _factory;
        }
        set
        {
            lock (_lock) { _factory = value ?? throw new ArgumentNullException(nameof(value)); }
        }
    }

    /// <summary>Initializes the ModelFactoryProvider with the specified factory.</summary>
    public static void Initialize(IModelFactory factory)
    { Current = factory ?? throw new ArgumentNullException(nameof(factory)); }
} 
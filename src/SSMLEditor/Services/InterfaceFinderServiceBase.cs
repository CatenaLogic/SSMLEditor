namespace SSMLEditor.Services;

using System;
using System.Collections.Generic;
using Catel.Logging;
using Catel.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public abstract class InterfaceFinderServiceBase<TInterface>
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(InterfaceFinderServiceBase<TInterface>));
    private readonly IServiceProvider _serviceProvider;

    protected InterfaceFinderServiceBase(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        _serviceProvider = serviceProvider;
    }

    protected IEnumerable<TInterface> GetAvailableItems()
    {
        // Note: don't cache

        var items = new List<TInterface>();

        var types = TypeCache.GetTypes(x => x.ImplementsInterfaceEx<TInterface>() && !x.IsAbstractEx());

        foreach (var type in types)
        {
            try
            {
                Logger.LogDebug("Found type '{TypeName}'", type.Name);

                var item = ActivatorUtilities.CreateInstance(_serviceProvider, type);
                if (item is TInterface typedItem)
                {
                    items.Add(typedItem);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to instantiate '{TypeName}'", type.FullName);
            }
        }

        return items;
    }
}

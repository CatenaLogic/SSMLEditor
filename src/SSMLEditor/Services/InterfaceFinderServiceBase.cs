namespace SSMLEditor.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;

    public abstract class InterfaceFinderServiceBase<TInterface>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected InterfaceFinderServiceBase()
        {

        }

        protected IEnumerable<TInterface> GetAvailableItems()
        {
            // Note: don't cache

            var items = new List<TInterface>();

            var typeFactory = this.GetTypeFactory();

            var types = TypeCache.GetTypes(x => x.ImplementsInterfaceEx<TInterface>() && !x.IsAbstractEx());

            foreach (var type in types)
            {
                try
                {
                    Log.Debug("Found type '{0}'", type.Name);

                    items.Add((TInterface)typeFactory.CreateInstance(type));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to instantiate '{0}'", type.FullName);
                }
            }

            return items;
        }
    }
}

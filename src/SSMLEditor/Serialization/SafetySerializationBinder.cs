namespace SSMLEditor.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Catel.Collections;
    using Catel.Reflection;
    using Newtonsoft.Json.Serialization;

    public class SafetySerializationBinder : ISerializationBinder
    {
        public SafetySerializationBinder()
        {
            // Only allow types from this app
            KnownTypes = new List<Type>();
            KnownTypes.AddRange(TypeCache.GetTypesOfAssembly(typeof(SSMLEditor.App).Assembly));
        }

        public IList<Type> KnownTypes { get; private set; }

        public Type BindToType(string assemblyName, string typeName)
        {
            var type = KnownTypes.FirstOrDefault(t => t.Name == typeName);
            if (type is null)
            {
                throw new InvalidOperationException($"Type '{typeName}' could not be found");
            }

            return type;
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }
    }
}

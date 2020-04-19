using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Console.Code
{
    public sealed class RegisteredPlugins
    {
        public IDictionary<RegisteredPluginsKey, List<RegisteredPluginValue>> Plugins { get; } =
            new Dictionary<RegisteredPluginsKey, List<RegisteredPluginValue>>();

        public RegisteredPlugins Add(Type serviceType, Type implementationType, string tenant, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var key = new RegisteredPluginsKey(serviceType, lifetime);
            if (!Plugins.TryGetValue(key, out var values))
            {
                values = new List<RegisteredPluginValue>();
                Plugins.Add(key, values);
            }
            values.Add(new RegisteredPluginValue(tenant, implementationType));
            return this;
        }

        public Type? Get(Type serviceType, string tenant)
        {
            var key = new RegisteredPluginsKey(serviceType);
            if (!Plugins.TryGetValue(key, out var values))
                return null;
            var matchingPlugin = values.FirstOrDefault(v => v.Tenant.Equals(tenant, StringComparison.OrdinalIgnoreCase));
            return matchingPlugin is { } ? matchingPlugin.ImplType : null;
        }

        internal IEnumerable<Type> GetAllImplTypes()
        {
            foreach (KeyValuePair<RegisteredPluginsKey, List<RegisteredPluginValue>> kvp in Plugins)
            {
                foreach (RegisteredPluginValue value in kvp.Value)
                    yield return value.ImplType;
            }
        }
    }
}
 
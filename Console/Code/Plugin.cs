using System;

namespace Console.Code
{
    internal sealed class Plugin<T> : IPlugin<T>
    {
        private readonly ITenantIdentificationStrategy _tenantIdentificationStrategy;
        private readonly RegisteredPlugins _registeredPlugins;
        private readonly IServiceProvider _serviceProvider;

        public Plugin(ITenantIdentificationStrategy tenantIdentificationStrategy,
            RegisteredPlugins registeredPlugins,
            IServiceProvider serviceProvider)
        {
            _tenantIdentificationStrategy = tenantIdentificationStrategy;
            _registeredPlugins = registeredPlugins;
            _serviceProvider = serviceProvider;
        }

        public T Get()
        {
            Type? implType = _registeredPlugins.Get(typeof(T), _tenantIdentificationStrategy.GetTenant());
            if (implType is null)
                throw new InvalidOperationException($"{typeof(T).FullName} is not registered as a plugin.");

            return (T)_serviceProvider.GetService(implType);
        }
    }
}
 
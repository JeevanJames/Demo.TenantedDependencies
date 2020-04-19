using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Console.Code
{
    public interface ITenantIdentificationStrategy
    {
        string? GetTenant();
    }

    public sealed class TenantIdentificationStrategy : ITenantIdentificationStrategy
    {
        private readonly IConfiguration _configuration;

        public TenantIdentificationStrategy(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string? GetTenant()
        {
            return _configuration["Tenant"] ?? "msft";
        }
    }

    public sealed class PluginDependency<T>
    {
        private readonly ITenantIdentificationStrategy _tenantIdentificationStrategy;
        private readonly RegisteredPlugins _registeredPlugins;
        private readonly IServiceProvider _serviceProvider;

        public PluginDependency(ITenantIdentificationStrategy tenantIdentificationStrategy,
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

    [DebuggerDisplay("{ServiceType.FullName}")]
    public readonly struct RegisteredPluginsKey
    {
        public RegisteredPluginsKey(Type serviceType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }

        public Type ServiceType { get; }

        public ServiceLifetime Lifetime { get; }

        public override int GetHashCode()
        {
            return HashCode.Combine(ServiceType);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (obj is RegisteredPluginsKey key)
                return ServiceType.Equals(key.ServiceType);
            return false;
        }
    }

    [DebuggerDisplay("{Tenant} ==> {ImplType.FullName}")]
    public sealed class RegisteredPluginValue
    {
        public RegisteredPluginValue(string tenant, Type implType)
        {
            Tenant = tenant;
            ImplType = implType;
        }

        public string Tenant { get; }

        public Type ImplType { get; }
    }

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

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlugins(this IServiceCollection services)
        {
            var plugins = new RegisteredPlugins();
            plugins.Add(typeof(IMyService), typeof(MicrosoftMyService), "msft");
            plugins.Add(typeof(IMyService), typeof(AmazonMyService), "amzn");
            services.AddSingleton(plugins);

            services.AddScoped<ITenantIdentificationStrategy, TenantIdentificationStrategy>();
            services.AddScoped(typeof(PluginDependency<>));

            IEnumerable<Type> implTypes = plugins.GetAllImplTypes().Distinct();
            foreach (Type implType in implTypes)
                services.AddScoped(implType);

            return services;
        }
    }

    public interface IMyService
    {
        string GetValue();
    }

    public sealed class MicrosoftMyService : IMyService
    {
        public string GetValue() => "Microsoft";
    }

    public sealed class AmazonMyService : IMyService
    {
        public string GetValue() => "Amazon";
    }
}
 
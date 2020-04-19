using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Console.Code
{
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
}
 
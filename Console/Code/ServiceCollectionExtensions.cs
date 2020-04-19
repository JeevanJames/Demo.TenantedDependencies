using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Console.Code
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlugins(this IServiceCollection services)
        {
            var plugins = new RegisteredPlugins();
            plugins.Add(typeof(IMyService), typeof(MicrosoftMyService), "msft");
            plugins.Add(typeof(IMyService), typeof(AmazonMyService), "amzn");
            plugins.Add(typeof(IMyService), typeof(GoogleMyService), "goog");
            services.AddSingleton(plugins);

            services.AddScoped<ITenantIdentificationStrategy, TenantIdentificationStrategy>();
            services.AddScoped(typeof(IPlugin<>), typeof(Plugin<>));

            IEnumerable<Type> implTypes = plugins.GetAllImplTypes().Distinct();
            foreach (Type implType in implTypes)
                services.AddScoped(implType);

            return services;
        }
    }
}
 
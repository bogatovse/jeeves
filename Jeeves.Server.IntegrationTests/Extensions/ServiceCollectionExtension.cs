using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Jeeves.Server.IntegrationTests.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void SwapTransient<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
        {
            SwapService(services, ServiceLifetime.Transient, implementationFactory);
        }
        
        public static void SwapSingleton<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
        {
            SwapService(services, ServiceLifetime.Singleton, implementationFactory);
        }
        
        public static void SwapScoped<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
        {
            SwapService(services, ServiceLifetime.Scoped, implementationFactory);
        }

        private static void SwapService<TService>(this IServiceCollection services, ServiceLifetime lifetime, Func<IServiceProvider, TService> implementationFactory)
        {
            if (services.Any(x => x.ServiceType == typeof(TService) && x.Lifetime == lifetime))
            {
                var serviceDescriptors = services.Where(x => x.ServiceType == typeof(TService) && x.Lifetime == lifetime).ToList();
                foreach (var serviceDescriptor in serviceDescriptors)
                {
                    services.Remove(serviceDescriptor);
                }
            }
            
            var descriptor = new ServiceDescriptor(typeof(TService),(sp) => implementationFactory(sp), lifetime);
            services.Add(descriptor);
        }
    }
}
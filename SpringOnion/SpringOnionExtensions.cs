using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpringOnion
{
    public static class SpringOnionExtensions
    {
        public static IServiceCollection ScanAssembly(this IServiceCollection services, Assembly assembly)
        {
            return services
                .AddAssemblyServices(assembly)
                .AddAssemblyBeans(assembly);
        }

        public static IServiceCollection AddAssemblyServices(this IServiceCollection services, Assembly assembly)
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            string environmentName = serviceProvider.GetRequiredService<IHostEnvironment>().EnvironmentName;

            foreach (Type type in assembly.GetExportedTypes())
            {
                foreach (ServiceDescriptor serviceDescriptor in ServiceRegistrator.GetServiceDescriptors(type, environmentName))
                {
                    services.Add(serviceDescriptor);
                }
            }

            return services;
        }

        public static IServiceCollection AddAssemblyBeans(this IServiceCollection services, Assembly assembly)
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            string environmentName = serviceProvider.GetRequiredService<IHostEnvironment>().EnvironmentName;

            foreach (Type type in assembly.GetExportedTypes())
            {
                foreach (ServiceDescriptor serviceDescriptor in BeanRegistrator.GetServiceDescriptors(type, environmentName))
                {
                    services.Add(serviceDescriptor);
                }
            }

            return services;
        }
    }
}

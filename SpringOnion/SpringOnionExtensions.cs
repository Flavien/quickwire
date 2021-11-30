using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace SpringOnion
{
    public static class SpringOnionExtensions
    {
        public static IServiceCollection ScanAssembly(
            this IServiceCollection services,
            Assembly assembly,
            ServiceDescriptorMergeStrategy mergeStrategy = ServiceDescriptorMergeStrategy.Replace)
        {
            return services
                .AddAssemblyServices(assembly, mergeStrategy)
                .AddAssemblyFactories(assembly, mergeStrategy);
        }

        public static IServiceCollection AddAssemblyServices(
            this IServiceCollection services,
            Assembly assembly,
            ServiceDescriptorMergeStrategy mergeStrategy)
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            string environmentName = serviceProvider.GetRequiredService<IHostEnvironment>().EnvironmentName;

            foreach (Type type in assembly.GetExportedTypes())
            {
                foreach (ServiceDescriptor serviceDescriptor in ServiceRegistrator.GetServiceDescriptors(type, environmentName))
                    MergeServiceDescriptor(services, serviceDescriptor, mergeStrategy);
            }

            return services;
        }

        public static IServiceCollection AddAssemblyFactories(
            this IServiceCollection services,
            Assembly assembly,
            ServiceDescriptorMergeStrategy mergeStrategy)
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            string environmentName = serviceProvider.GetRequiredService<IHostEnvironment>().EnvironmentName;

            foreach (Type type in assembly.GetExportedTypes())
            {
                foreach (ServiceDescriptor serviceDescriptor in FactoryRegistrator.GetServiceDescriptors(type, environmentName))
                    MergeServiceDescriptor(services, serviceDescriptor, mergeStrategy);
            }

            return services;
        }

        public static IServiceCollection AddType(
            this IServiceCollection services,
            Type type, ServiceDescriptorMergeStrategy
            mergeStrategy = ServiceDescriptorMergeStrategy.Replace)
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            string environmentName = serviceProvider.GetRequiredService<IHostEnvironment>().EnvironmentName;

            foreach (ServiceDescriptor serviceDescriptor in ServiceRegistrator.GetServiceDescriptors(type, environmentName))
                MergeServiceDescriptor(services, serviceDescriptor, mergeStrategy);

            foreach (ServiceDescriptor serviceDescriptor in FactoryRegistrator.GetServiceDescriptors(type, environmentName))
                MergeServiceDescriptor(services, serviceDescriptor, mergeStrategy);

            return services;
        }

        private static void MergeServiceDescriptor(
            IServiceCollection services,
            ServiceDescriptor serviceDescriptor,
            ServiceDescriptorMergeStrategy mergeStrategy)
        {
            switch (mergeStrategy)
            {
                case ServiceDescriptorMergeStrategy.Replace:
                    services.Replace(serviceDescriptor);
                    break;
                case ServiceDescriptorMergeStrategy.Skip:
                    services.TryAdd(serviceDescriptor);
                    break;
                default:
                    if (services.Any(service => service.ServiceType == serviceDescriptor.ServiceType))
                        throw new ArgumentException(
                            $"The service of type {serviceDescriptor.ServiceType.FullName} has already been added.",
                            nameof(serviceDescriptor));

                    services.Replace(serviceDescriptor);
                    break;
            }
        }
    }
}

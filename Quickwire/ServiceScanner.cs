using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quickwire.Attributes;

namespace Quickwire
{
    public static class ServiceScanner
    {
        public static IEnumerable<ServiceDescriptor> ScanServiceRegistrations(Type type, IServiceProvider serviceProvider)
        {
            IServiceActivator serviceActivator = serviceProvider.GetService<IServiceActivator>();

            if (CanScan(type, serviceProvider))
            {
                foreach (RegisterServiceAttribute registerAttribute in type.GetCustomAttributes<RegisterServiceAttribute>())
                {
                    Type serviceType = registerAttribute.ServiceType ?? type;
                    yield return new ServiceDescriptor(serviceType, serviceActivator.GetFactory(type), registerAttribute.Scope);
                }
            }
        }

        public static IEnumerable<ServiceDescriptor> ScanFactoryRegistrations(Type type, IServiceProvider serviceProvider)
        {
            if (CanScan(type, serviceProvider))
            {
                IServiceActivator serviceActivator = serviceProvider.GetService<IServiceActivator>();

                foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    if (CanScan(method, serviceProvider))
                    {
                        foreach (RegisterFactoryAttribute registerAttribute in method.GetCustomAttributes<RegisterFactoryAttribute>())
                        {
                            Type serviceType = registerAttribute.ServiceType ?? type;

                            yield return new ServiceDescriptor(serviceType, serviceActivator.GetFactory(method), registerAttribute.Scope);
                        }
                    }
                }
            }
        }

        private static bool CanScan(ICustomAttributeProvider customAttributeProvider, IServiceProvider serviceProvider)
        {
            return customAttributeProvider
                .GetCustomAttributes(typeof(IServiceScanningFilter), false)
                .OfType<IServiceScanningFilter>()
                .All(filter => filter.CanScan(serviceProvider));
        }
    }
}

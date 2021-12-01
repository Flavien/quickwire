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
            if (CanScan(type, serviceProvider))
            {
                foreach (RegisterServiceAttribute registerAttribute in type.GetCustomAttributes<RegisterServiceAttribute>())
                {
                    Type serviceType = registerAttribute.ServiceType ?? type;
                    yield return new ServiceDescriptor(serviceType, ServiceRegistrator.GetFactory(type), registerAttribute.Scope);
                }
            }
        }

        public static IEnumerable<ServiceDescriptor> ScanFactoryRegistrations(Type type, IServiceProvider serviceProvider)
        {
            if (CanScan(type, serviceProvider))
                return ScanFactoryMethods(type, serviceProvider);
            else
                return Enumerable.Empty<ServiceDescriptor>();
        }

        private static IEnumerable<ServiceDescriptor> ScanFactoryMethods(Type type, IServiceProvider serviceProvider)
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                if (CanScan(method, serviceProvider))
                {
                    foreach (RegisterFactoryAttribute registerAttribute in method.GetCustomAttributes<RegisterFactoryAttribute>())
                    {
                        Type serviceType = registerAttribute.ServiceType ?? type;

                        yield return new ServiceDescriptor(serviceType, FactoryRegistrator.GetFactory(method), registerAttribute.Scope);
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

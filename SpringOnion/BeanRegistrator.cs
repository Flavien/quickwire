using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpringOnion.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpringOnion
{
    public static class BeanRegistrator
    {
        public static IEnumerable<ServiceDescriptor> GetServiceDescriptors(Type type, string environmentName)
        {
            if (EnvironmentSelectorAttribute.IsEnabled(type.GetCustomAttribute<EnvironmentSelectorAttribute>(), environmentName))
            {
                return ScanBeans(type, environmentName);
            }
            else
            {
                return Enumerable.Empty<ServiceDescriptor>();
            }
        }

        private static IEnumerable<ServiceDescriptor> ScanBeans(Type type, string environmentName)
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                if (EnvironmentSelectorAttribute.IsEnabled(method.GetCustomAttribute<EnvironmentSelectorAttribute>(), environmentName))
                {
                    foreach (RegisterBeanAttribute registerAttribute in method.GetCustomAttributes<RegisterBeanAttribute>())
                    {
                        Type serviceType = registerAttribute.ServiceType ?? type;

                        yield return new ServiceDescriptor(serviceType, GetFactory(method), registerAttribute.Scope);
                    }
                }
            }
        }

        private static Func<IServiceProvider, object?> GetFactory(MethodInfo methodInfo)
        {
            ParameterInfo[]? parameters = methodInfo.GetParameters();
            DependencyResolverAttribute?[] dependencyResolvers = new DependencyResolverAttribute[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                dependencyResolvers[i] = parameters[i].GetCustomAttribute<DependencyResolverAttribute>();
            }

            return delegate (IServiceProvider serviceProvider)
            {
                object[] arguments = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    arguments[i] = DependencyResolverAttribute.Resolve(
                        serviceProvider,
                        parameters[i].ParameterType,
                        dependencyResolvers[i]);
                }

                object? result = methodInfo.Invoke(null, arguments);

                return result;
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Quickwire.Attributes;

namespace Quickwire
{
    public static class ServiceRegistrator
    {
        public static IEnumerable<ServiceDescriptor> GetServiceDescriptors(Type type, string environmentName)
        {
            if (EnvironmentSelectorAttribute.IsEnabled(type.GetCustomAttribute<EnvironmentSelectorAttribute>(), environmentName))
            {
                foreach (RegisterServiceAttribute registerAttribute in type.GetCustomAttributes<RegisterServiceAttribute>())
                {
                    Type serviceType = registerAttribute.ServiceType ?? type;
                    yield return new ServiceDescriptor(serviceType, GetFactory(type), registerAttribute.Scope);
                }
            }
        }

        public static Func<IServiceProvider, object> GetFactory(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            ConstructorInfo constructor;
            if (constructors.Length == 1)
            {
                constructor = constructors[0];
            }
            else
            {
                List<ConstructorInfo> primaryConstructor = constructors
                    .Where(constructor => constructor.IsDefined(typeof(ServiceConstructorAttribute), true))
                    .ToList();

                if (primaryConstructor.Count != 1)
                    throw new ArgumentException();
                else
                    constructor = primaryConstructor[0];
            }

            ParameterInfo[]? parameters = constructor.GetParameters();
            DependencyResolverAttribute?[] dependencyResolvers = new DependencyResolverAttribute[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                dependencyResolvers[i] = parameters[i].GetCustomAttribute<DependencyResolverAttribute>();
            }

            bool injectAllInitOnlyProperties = type.IsDefined(typeof(InjectAllInitOnlyPropertiesAttribute), true);

            List<SetterInfo> setters = new List<SetterInfo>();

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                DependencyResolverAttribute? dependencyResolver = property.GetCustomAttribute<DependencyResolverAttribute>();
                MethodInfo? setter = property.SetMethod;

                if (setter != null)
                {
                    bool isInitOnly = setter.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(IsExternalInit));

                    if (dependencyResolver != null || (injectAllInitOnlyProperties && isInitOnly))
                        setters.Add(new SetterInfo(property.PropertyType, setter, dependencyResolver));
                }
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

                object result = constructor.Invoke(arguments);

                foreach (SetterInfo setter in setters)
                {
                    object resolvedDependency = DependencyResolverAttribute.Resolve(
                        serviceProvider,
                        setter.ServiceType,
                        setter.DependencyResolver);

                    setter.Setter.Invoke(result, new[] { resolvedDependency });
                }

                return result;
            };
        }

        private record SetterInfo(Type ServiceType, MethodInfo Setter, DependencyResolverAttribute? DependencyResolver);
    }
}

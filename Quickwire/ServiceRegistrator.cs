// Copyright 2021 Flavien Charlon
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
            ConstructorInfo constructor = GetConstructor(type);
            ParameterInfo[] parameters = constructor.GetParameters();
            DependencyResolverAttribute?[] dependencyResolvers = GetConstructorDependencyResolvers(parameters);
            List<SetterInfo> setters = GetSetters(type);

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

        private static ConstructorInfo GetConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            if (constructors.Length == 1)
            {
                return constructors[0];
            }
            else
            {
                List<ConstructorInfo> primaryConstructor = constructors
                    .Where(constructor => constructor.IsDefined(typeof(ServiceConstructorAttribute), true))
                    .ToList();

                if (primaryConstructor.Count != 1)
                    throw new ArgumentException();
                else
                    return primaryConstructor[0];
            }
        }

        private static DependencyResolverAttribute?[] GetConstructorDependencyResolvers(ParameterInfo[] parameters)
        {
            DependencyResolverAttribute?[] dependencyResolvers = new DependencyResolverAttribute[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                dependencyResolvers[i] = parameters[i].GetCustomAttribute<DependencyResolverAttribute>();

            return dependencyResolvers;
        }

        private static List<SetterInfo> GetSetters(Type type)
        {
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

            return setters;
        }

        private record SetterInfo(Type ServiceType, MethodInfo Setter, DependencyResolverAttribute? DependencyResolver);
    }
}

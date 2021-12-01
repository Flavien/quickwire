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
    public static class ServiceActivator
    {
        public static Func<IServiceProvider, object?> GetFactory(MethodInfo methodInfo)
        {
            ParameterInfo[]? parameters = methodInfo.GetParameters();
            IDependencyResolver?[] dependencyResolvers = GetParametersDependencyResolvers(parameters);

            return delegate (IServiceProvider serviceProvider)
            {
                object[] arguments = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                    arguments[i] = Resolve(serviceProvider, parameters[i].ParameterType, dependencyResolvers[i]);

                object? result = methodInfo.Invoke(null, arguments);

                return result;
            };
        }

        public static Func<IServiceProvider, object> GetFactory(Type type)
        {
            ConstructorInfo constructor = GetConstructor(type);
            ParameterInfo[] parameters = constructor.GetParameters();
            IDependencyResolver?[] dependencyResolvers = GetParametersDependencyResolvers(parameters);
            List<SetterInfo> setters = GetSetters(type);

            return delegate (IServiceProvider serviceProvider)
            {
                object[] arguments = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                    arguments[i] = Resolve(serviceProvider, parameters[i].ParameterType, dependencyResolvers[i]);

                object result = constructor.Invoke(arguments);

                foreach (SetterInfo setter in setters)
                {
                    object resolvedDependency = Resolve(serviceProvider, setter.ServiceType, setter.DependencyResolver);

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

        private static IDependencyResolver?[] GetParametersDependencyResolvers(ParameterInfo[] parameters)
        {
            IDependencyResolver?[] dependencyResolvers = new IDependencyResolver[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                dependencyResolvers[i] = GetDependencyResolver(parameters[i]);

            return dependencyResolvers;
        }

        private static List<SetterInfo> GetSetters(Type type)
        {
            bool injectAllInitOnlyProperties = type.IsDefined(typeof(InjectAllInitOnlyPropertiesAttribute), true);

            List<SetterInfo> setters = new List<SetterInfo>();

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                IDependencyResolver? dependencyResolver = GetDependencyResolver(property);
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

        private static IDependencyResolver? GetDependencyResolver(ICustomAttributeProvider customAttributeProvider)
        {
            return customAttributeProvider
                .GetCustomAttributes(typeof(IDependencyResolver), true)
                .OfType<IDependencyResolver>()
                .FirstOrDefault();
        }

        private static object Resolve(IServiceProvider serviceProvider, Type serviceType, IDependencyResolver? dependencyResolver)
        {
            if (dependencyResolver == null)
                return serviceProvider.GetRequiredService(serviceType);
            else
                return dependencyResolver.Resolve(serviceProvider, serviceType);
        }

        private record SetterInfo(Type ServiceType, MethodInfo Setter, IDependencyResolver? DependencyResolver);
    }
}

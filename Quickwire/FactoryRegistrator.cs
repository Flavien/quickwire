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
using Microsoft.Extensions.DependencyInjection;
using Quickwire.Attributes;

namespace Quickwire
{
    public static class FactoryRegistrator
    {
        public static IEnumerable<ServiceDescriptor> GetServiceDescriptors(Type type, string environmentName)
        {
            if (EnvironmentSelectorAttribute.IsEnabled(type.GetCustomAttribute<EnvironmentSelectorAttribute>(), environmentName))
                return ScanFactoryMethods(type, environmentName);
            else
                return Enumerable.Empty<ServiceDescriptor>();
        }

        private static IEnumerable<ServiceDescriptor> ScanFactoryMethods(Type type, string environmentName)
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                if (EnvironmentSelectorAttribute.IsEnabled(method.GetCustomAttribute<EnvironmentSelectorAttribute>(), environmentName))
                {
                    foreach (RegisterFactoryAttribute registerAttribute in method.GetCustomAttributes<RegisterFactoryAttribute>())
                    {
                        Type serviceType = registerAttribute.ServiceType ?? type;

                        yield return new ServiceDescriptor(serviceType, GetFactory(method), registerAttribute.Scope);
                    }
                }
            }
        }

        public static Func<IServiceProvider, object?> GetFactory(MethodInfo methodInfo)
        {
            ParameterInfo[]? parameters = methodInfo.GetParameters();
            DependencyResolverAttribute?[] dependencyResolvers = GetFactoryDependencyResolvers(parameters);

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

        private static DependencyResolverAttribute?[] GetFactoryDependencyResolvers(ParameterInfo[] parameters)
        {
            DependencyResolverAttribute?[] dependencyResolvers = new DependencyResolverAttribute[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                dependencyResolvers[i] = parameters[i].GetCustomAttribute<DependencyResolverAttribute>();
            }

            return dependencyResolvers;
        }
    }
}

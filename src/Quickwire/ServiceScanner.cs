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
    public static class ServiceScanner
    {
        public static IEnumerable<ServiceDescriptor> ScanServiceRegistrations(Type type, IServiceProvider serviceProvider)
        {
            IServiceActivator serviceActivator = serviceProvider.GetService<IServiceActivator>();

            if (!type.IsAbstract && CanScan(type, serviceProvider))
            {
                foreach (RegisterServiceAttribute registerAttribute in type.GetCustomAttributes<RegisterServiceAttribute>())
                {
                    Type serviceType = registerAttribute.ServiceType ?? type;
                    yield return new ServiceDescriptor(
                        serviceType,
                        serviceActivator.GetFactory(type),
                        registerAttribute.Scope);
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
                            Type serviceType = registerAttribute.ServiceType ?? method.ReturnType;
                            yield return new ServiceDescriptor(
                                serviceType,
                                serviceActivator.GetFactory(method),
                                registerAttribute.Scope);
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

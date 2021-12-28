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

namespace Quickwire;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ScanAssembly(
        this IServiceCollection services,
        Assembly assembly,
        Func<Type, bool> typeFilter,
        ServiceDescriptorMergeStrategy mergeStrategy = ServiceDescriptorMergeStrategy.Replace)
    {
        services.ScanTypes(assembly.GetExportedTypes().Where(typeFilter), mergeStrategy);

        return services;
    }

    public static IServiceCollection ScanCurrentAssembly(
        this IServiceCollection services,
        ServiceDescriptorMergeStrategy mergeStrategy = ServiceDescriptorMergeStrategy.Replace)
    {
        return services.ScanAssembly(Assembly.GetCallingAssembly(), static _ => true, mergeStrategy);
    }

    public static IServiceCollection ScanTypes(
        this IServiceCollection services,
        IEnumerable<Type> types,
        ServiceDescriptorMergeStrategy mergeStrategy = ServiceDescriptorMergeStrategy.Replace)
    {
        services.TryAddSingleton<IServiceActivator>(new ServiceActivator());
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        IEnumerable<Func<ServiceDescriptor>> serviceDescriptorFactoryList =
            types.SelectMany(type =>
                ServiceScanner.ScanServiceRegistrations(type, serviceProvider).Concat(
                ServiceScanner.ScanFactoryRegistrations(type, serviceProvider)));

        List<ServiceDescriptor> serviceDescriptors = new();
        object gate = new();

        Parallel.ForEach(
            serviceDescriptorFactoryList,
            getDescriptor =>
            {
                ServiceDescriptor descriptor = getDescriptor();
                lock (gate)
                    serviceDescriptors.Add(descriptor);
            });

        foreach (ServiceDescriptor serviceDescriptor in serviceDescriptors)
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
                        $"The service of type {serviceDescriptor.ServiceType.FullName} has already been added.");

                services.Replace(serviceDescriptor);
                break;
        }
    }
}

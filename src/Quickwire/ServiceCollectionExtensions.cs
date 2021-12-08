﻿// Copyright 2021 Flavien Charlon
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
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ScanAssembly(
        this IServiceCollection services,
        Assembly assembly,
        ServiceDescriptorMergeStrategy mergeStrategy = ServiceDescriptorMergeStrategy.Replace)
    {
        foreach (Type type in assembly.GetExportedTypes())
            services.ScanType(type, mergeStrategy);

        return services;
    }

    public static IServiceCollection ScanCurrentAssembly(
        this IServiceCollection services,
        ServiceDescriptorMergeStrategy mergeStrategy = ServiceDescriptorMergeStrategy.Replace)
    {
        return services.ScanAssembly(Assembly.GetCallingAssembly(), mergeStrategy);
    }

    public static IServiceCollection ScanType(
        this IServiceCollection services,
        Type type, ServiceDescriptorMergeStrategy
        mergeStrategy = ServiceDescriptorMergeStrategy.Replace)
    {
        services.TryAddSingleton<IServiceActivator>(new ServiceActivator());
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        foreach (ServiceDescriptor serviceDescriptor in ServiceScanner.ScanServiceRegistrations(type, serviceProvider))
            MergeServiceDescriptor(services, serviceDescriptor, mergeStrategy);

        foreach (ServiceDescriptor serviceDescriptor in ServiceScanner.ScanFactoryRegistrations(type, serviceProvider))
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

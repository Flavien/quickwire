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
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Quickwire.Attributes;

/// <summary>
/// Represents a class that can construct factory methods based either on type constructors or static methods.
/// </summary>
public class ServiceActivator : IServiceActivator
{
    /// <summary>
    /// Returns a factory method based on a static method.
    /// </summary>
    public Func<IServiceProvider, object?> GetFactory(MethodInfo methodInfo)
    {
        if (!methodInfo.IsStatic)
            throw new InvalidOperationException("Factory methods must be static.");

        ParameterInfo[]? parameters = methodInfo.GetParameters();
        IDependencyResolver?[] dependencyResolvers = GetParametersDependencyResolvers(parameters);
        DelegateCompiler.Factory factory = DelegateCompiler.CreateFactory(methodInfo);

        return delegate (IServiceProvider serviceProvider)
        {
            object?[] arguments = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                arguments[i] = Resolve(serviceProvider, parameters[i].ParameterType, dependencyResolvers[i]);

            object? result = factory(arguments);

            return result;
        };
    }

    /// <summary>
    /// Returns a factory method based on a type constructor.
    /// </summary>
    public Func<IServiceProvider, object> GetFactory(Type type)
    {
        ConstructorInfo constructorInfo = GetConstructor(type);
        ParameterInfo[] parameters = constructorInfo.GetParameters();
        IDependencyResolver?[] dependencyResolvers = GetParametersDependencyResolvers(parameters);
        DelegateCompiler.Constructor constructor = DelegateCompiler.CreateConstructor(constructorInfo);
        List<SetterInfo> setters = GetSetters(type);

        return delegate (IServiceProvider serviceProvider)
        {
            object?[] arguments = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                arguments[i] = Resolve(serviceProvider, parameters[i].ParameterType, dependencyResolvers[i]);

            object result = constructor(arguments);

            foreach (SetterInfo setter in setters)
            {
                object? resolvedDependency = Resolve(serviceProvider, setter.ServiceType, setter.DependencyResolver);
                setter.Setter(result, resolvedDependency);
            }

            return result;
        };
    }

    private static ConstructorInfo GetConstructor(Type type)
    {
        ConstructorInfo[] constructors = type.GetConstructors(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        List<ConstructorInfo> primaryConstructor = constructors
            .Where(constructor => constructor.IsDefined(typeof(ServiceConstructorAttribute), false))
            .ToList();

        if (primaryConstructor.Count == 1)
        {
            return primaryConstructor[0];
        }
        else if (primaryConstructor.Count > 1)
        {
            throw new ArgumentException(
                $"The type {type.FullName} has more than one constructor decorated with the " +
                $"[ServiceConstructor] attribute.");
        }
        else
        {
            List<ConstructorInfo> publicConstructors = constructors
                .Where(constructors => constructors.IsPublic)
                .ToList();

            if (publicConstructors.Count == 1)
                return publicConstructors[0];
            else
                throw new ArgumentException($"The type {type.FullName} must have exactly one public constructor.");
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
        PropertyInfo[] properties = type.GetProperties(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            IDependencyResolver? dependencyResolver = GetDependencyResolver(property);
            MethodInfo? setter = property.SetMethod;

            if (setter != null)
            {
                bool isInitOnly = setter.ReturnParameter
                    .GetRequiredCustomModifiers()
                    .Contains(typeof(IsExternalInit));

                if (dependencyResolver != null || (injectAllInitOnlyProperties && isInitOnly))
                {
                    DelegateCompiler.Setter compiledSetter = DelegateCompiler.CreateSetter(type, setter);
                    setters.Add(new SetterInfo(property.PropertyType, compiledSetter, dependencyResolver));
                }
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

    private static object? Resolve(IServiceProvider serviceProvider, Type serviceType, IDependencyResolver? dependencyResolver)
    {
        if (dependencyResolver == null)
            return serviceProvider.GetRequiredService(serviceType);
        else
            return dependencyResolver.Resolve(serviceProvider, serviceType);
    }

    private record SetterInfo(Type ServiceType, DelegateCompiler.Setter Setter, IDependencyResolver? DependencyResolver);
}

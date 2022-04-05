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

namespace Quickwire.Attributes;

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Specifies that a class is a service that can be used for dependency injection.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RegisterServiceAttribute : Attribute
{
    public RegisterServiceAttribute(ServiceLifetime scope)
    {
        Scope = scope;
    }

    /// <summary>
    /// The lifetime of the service.
    /// </summary>
    public ServiceLifetime Scope { get; set; }

    /// <summary>
    /// The type of the service.
    /// </summary>
    public Type? ServiceType { get; set; }
}

/// <summary>
/// Specifies that a class is a IHostedService that will be registered as Singleton in DI container
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RegisterBackgroundAttribute : RegisterServiceAttribute
{
    public RegisterBackgroundAttribute() : base(ServiceLifetime.Singleton)
    {
        ServiceType = typeof(IHostedService);
    }
}

/// <summary>
/// Specifies that a class will be registered as Singleton in DI container.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RegisterSingletonAttribute : RegisterServiceAttribute
{
    public RegisterSingletonAttribute(Type? serviceType = null) : base(ServiceLifetime.Singleton)
    {
        ServiceType = serviceType;
    }
}

/// <summary>
/// Specifies that a class will be registered as Scoped in DI container.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RegisterScopedAttribute : RegisterServiceAttribute
{
    public RegisterScopedAttribute(Type? serviceType = null) : base(ServiceLifetime.Scoped)
    {
        ServiceType = serviceType;
    }
}

/// <summary>
/// Specifies that a class will be registered as Transient in DI container.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RegisterTransientAttribute : RegisterServiceAttribute
{
    public RegisterTransientAttribute(Type? serviceType = null) : base(ServiceLifetime.Transient)
    {
        ServiceType = serviceType;
    }
}

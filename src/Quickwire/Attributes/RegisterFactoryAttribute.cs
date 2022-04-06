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

namespace Quickwire.Attributes;

using System;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Specifies that a method is a factory producing a service that can be used for dependency injection.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RegisterFactoryAttribute : Attribute
{
    public RegisterFactoryAttribute(ServiceLifetime scope)
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

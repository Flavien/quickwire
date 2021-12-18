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

/// <summary>
/// Specifies that a parameter or property should be bound using dependency injection resolution.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class InjectServiceAttribute : Attribute, IDependencyResolver
{
    /// <summary>
    /// Gets or sets a boolean value indicating whether the service is required or optional.
    /// </summary>
    public bool Optional { get; set; }

    public object? Resolve(IServiceProvider serviceProvider, Type type)
    {
        if (Optional)
            return serviceProvider.GetService(type);
        else
            return serviceProvider.GetRequiredService(type);
    }
}

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
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Specifies that a type or method should be excluded or included from dependency injection registration based on
/// the current environment specified throught the <see cref="IHostEnvironment"/> service.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class EnvironmentSelectorAttribute : Attribute, IServiceScanningFilter
{
    public string[]? Enabled { get; set; }

    public string[]? Disabled { get; set; }

    public bool CanScan(IServiceProvider serviceProvider)
    {
        string environmentName = serviceProvider.GetRequiredService<IHostEnvironment>().EnvironmentName;

        bool enabled = true;
        if (Enabled != null)
            enabled &= Enabled.Contains(environmentName, StringComparer.OrdinalIgnoreCase);

        if (Disabled != null)
            enabled &= !Disabled.Contains(environmentName, StringComparer.OrdinalIgnoreCase);

        return enabled;
    }
}

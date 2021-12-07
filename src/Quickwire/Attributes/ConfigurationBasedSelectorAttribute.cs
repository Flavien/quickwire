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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Quickwire.Attributes
{
    /// <summary>
    /// Indicates that a type or method should be excluded or included from dependency injection registration based
    /// on a configuration setting coming from the <see cref="IConfiguration"/> service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ConfigurationBasedSelectorAttribute : Attribute, IServiceScanningFilter
    {
        private readonly string _configurationKey;
        private readonly string _enabledIfEqualsTo;

        public ConfigurationBasedSelectorAttribute(string configurationKey, string enabledIfEqualsTo)
        {
            _configurationKey = configurationKey;
            _enabledIfEqualsTo = enabledIfEqualsTo;
        }

        public bool CanScan(IServiceProvider serviceProvider)
        {
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
            string value = configuration[_configurationKey];

            return StringComparer.OrdinalIgnoreCase.Equals(value, _enabledIfEqualsTo);
        }
    }
}

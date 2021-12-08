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
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Indicates that a parameter or property should be bound using a configuration setting coming from the
/// <see cref="IConfiguration"/> service.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class InjectConfigurationAttribute : Attribute, IDependencyResolver
{
    public InjectConfigurationAttribute(string configurationKey)
    {
        ConfigurationKey = configurationKey;
    }

    /// <summary>
    /// Gets or sets the configuration key to use.
    /// </summary>
    public string ConfigurationKey { get; set; }

    public object? Resolve(IServiceProvider serviceProvider, Type type)
    {
        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
        string value = configuration[ConfigurationKey];

        Type? nullableOf = Nullable.GetUnderlyingType(type);

        if (nullableOf != null)
            return value == null ? null : Resolve(serviceProvider, nullableOf);
        if (type == typeof(TimeSpan))
            return TimeSpan.Parse(value);
        if (type == typeof(Guid))
            return Guid.Parse(value);
        if (type == typeof(Uri))
            return new Uri(value);
        else if (type.IsEnum)
            return Enum.Parse(type, value);
        else
        {
            try
            {
                return Convert.ChangeType(value, type);
            }
            catch (InvalidCastException)
            {
                object? result = TryParse(type, value);
                if (result != null)
                    return result;
                else
                    throw;
            }
        }
    }

    private static object? TryParse(Type type, string value)
    {
        MethodInfo? parse = type.GetMethod(
            "Parse",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new[] { typeof(string) },
            new ParameterModifier[0]);

        if (parse != null && !parse.IsGenericMethod)
            return parse.Invoke(null, new object[] { value });
        else
            return null;
    }
}

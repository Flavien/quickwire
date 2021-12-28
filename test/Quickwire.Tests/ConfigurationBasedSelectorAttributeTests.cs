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

namespace Quickwire.Tests;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quickwire.Attributes;
using Xunit;

public class ConfigurationBasedSelectorAttributeTests
{
    private readonly IServiceProvider _serviceProvider;

    public ConfigurationBasedSelectorAttributeTests()
    {
        ServiceCollection services = new();
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        configuration["key"] = "value";
        services.AddSingleton<IConfiguration>(configuration);
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void CanScan_Enabled()
    {
        ConfigurationBasedSelectorAttribute selector = new("key", "value");

        bool canScan = selector.CanScan(_serviceProvider);

        Assert.True(canScan);
    }

    [Fact]
    public void CanScan_Disabled()
    {
        ConfigurationBasedSelectorAttribute selector = new("key", "no");

        bool canScan = selector.CanScan(_serviceProvider);

        Assert.False(canScan);
    }
}

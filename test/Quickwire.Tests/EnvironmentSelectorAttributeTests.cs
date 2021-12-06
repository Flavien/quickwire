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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quickwire.Attributes;
using Quickwire.Tests.Implementations;
using Xunit;

namespace Quickwire.Tests
{
    public class EnvironmentSelectorAttributeTests
    {
        private readonly IServiceProvider _serviceProvider;

        public EnvironmentSelectorAttributeTests()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IHostEnvironment>(new MockHostEnvironment() { EnvironmentName = "B" });
            _serviceProvider = services.BuildServiceProvider();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("B", null)]
        [InlineData("A,B,C", null)]
        [InlineData("A,B,C", "D,E,F")]
        public void CanScan_Enabled(string enabled, string disabled)
        {
            EnvironmentSelectorAttribute environmentSelector = new EnvironmentSelectorAttribute();
            environmentSelector.Enabled = enabled == null ? null : enabled.Split(',');
            environmentSelector.Disabled = disabled == null ? null : disabled.Split(',');

            bool canScan = environmentSelector.CanScan(_serviceProvider);

            Assert.True(canScan);
        }

        [Theory]
        [InlineData(null, "B")]
        [InlineData("D,E,F", "A,B,C")]
        [InlineData("A,B,C", "A,B,C")]
        public void CanScan_Disabled(string enabled, string disabled)
        {
            EnvironmentSelectorAttribute environmentSelector = new EnvironmentSelectorAttribute();
            environmentSelector.Enabled = enabled == null ? null : enabled.Split(',');
            environmentSelector.Disabled = disabled == null ? null : disabled.Split(',');

            bool canScan = environmentSelector.CanScan(_serviceProvider);

            Assert.False(canScan);
        }
    }
}

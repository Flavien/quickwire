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
using Quickwire.Attributes;
using Quickwire.Tests.Implementations;
using Xunit;

namespace Quickwire.Tests
{
    public class InjectServiceAttributeTests
    {
        private readonly IServiceProvider _serviceProvider;

        public InjectServiceAttributeTests()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<Dependency>(new Dependency("Default"));
            _serviceProvider = services.BuildServiceProvider();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Resolve_Found(bool optional)
        {
            InjectServiceAttribute injectService = new InjectServiceAttribute() { Optional = optional };

            Dependency resolvedDependency = injectService.Resolve(_serviceProvider, typeof(Dependency)) as Dependency;

            Assert.NotNull(resolvedDependency);
            Assert.Equal("Default", resolvedDependency.Value);
        }

        [Fact]
        public void Resolve_NotFoundRequired()
        {
            InjectServiceAttribute injectService = new InjectServiceAttribute();

            Assert.Throws<InvalidOperationException>(() =>
                injectService.Resolve(_serviceProvider, typeof(IComparable)));
        }

        [Fact]
        public void Resolve_NotFoundOptional()
        {
            InjectServiceAttribute injectService = new InjectServiceAttribute() { Optional = true };

            object resolvedDependency = injectService.Resolve(_serviceProvider, typeof(IComparable));

            Assert.Null(resolvedDependency);
        }
    }
}

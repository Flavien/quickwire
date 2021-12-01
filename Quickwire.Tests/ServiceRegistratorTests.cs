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
using Xunit;

namespace Quickwire.Tests
{
    public class ServiceRegistratorTests
    {
        private readonly ServiceCollection _services;
        private readonly IServiceProvider _serviceProvider;
        private readonly TestObjects.Dependency _dependency = new TestObjects.Dependency();

        public ServiceRegistratorTests()
        {
            _services = new ServiceCollection();
            _services.AddSingleton<TestObjects.Dependency>(_dependency);
            _services.AddSingleton<string>("Test");
            _serviceProvider = _services.BuildServiceProvider();
        }

        [Fact]
        public void GetFactory_ConstructorInjection()
        {
            object resultObject = ServiceRegistrator.GetFactory(typeof(TestObjects.ConstructorInjection))(_serviceProvider);
            TestObjects.ConstructorInjection result = resultObject as TestObjects.ConstructorInjection;

            Assert.NotNull(result);
            Assert.Equal(_dependency, result.Dependency1);
            Assert.Equal("Test", result.Dependency2);
        }

        [Fact]
        public void GetFactory_MultipleConstructors()
        {
            object resultObject = ServiceRegistrator.GetFactory(typeof(TestObjects.MultipleConstructors))(_serviceProvider);
            TestObjects.MultipleConstructors result = resultObject as TestObjects.MultipleConstructors;

            Assert.NotNull(result);
            Assert.Equal(_dependency, result.Dependency1);
            Assert.Equal("Second Constructor", result.Dependency2);
        }

        [Fact]
        public void GetFactory_NoSetterInjection()
        {
            object resultObject = ServiceRegistrator.GetFactory(typeof(TestObjects.NoSetterInjection))(_serviceProvider);
            TestObjects.NoSetterInjection result = resultObject as TestObjects.NoSetterInjection;

            Assert.NotNull(result);
            Assert.Null(result.DependencyGet);
            Assert.Null(result.DependencyGetSet);
            Assert.Null(result.DependencyGetInit);
        }

        [Fact]
        public void GetFactory_SetterInjection()
        {
            object resultObject = ServiceRegistrator.GetFactory(typeof(TestObjects.SetterInjection))(_serviceProvider);
            TestObjects.SetterInjection result = resultObject as TestObjects.SetterInjection;

            Assert.NotNull(result);
            Assert.Equal(_dependency, result.DependencyGetSet);
        }

        [Fact]
        public void GetFactory_InitOnlySetterInjection()
        {
            object resultObject = ServiceRegistrator.GetFactory(typeof(TestObjects.InitOnlySetterInjection))(_serviceProvider);
            TestObjects.InitOnlySetterInjection result = resultObject as TestObjects.InitOnlySetterInjection;

            Assert.NotNull(result);
            Assert.Null(result.DependencyGet);
            Assert.Null(result.DependencyGetSet);
            Assert.Equal(_dependency, result.DependencyGetInit);
        }
    }
}

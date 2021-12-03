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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quickwire.Tests.Implementations;
using Xunit;

namespace Quickwire.Tests
{
    public partial class ServiceCollectionExtensionsTests
    {
        private readonly ServiceCollection _services;
        private readonly IServiceProvider _serviceProvider;

        public ServiceCollectionExtensionsTests()
        {
            _services = new ServiceCollection();
            _services.AddSingleton<IServiceActivator>(new MockServiceActivator());
            _serviceProvider = _services.BuildServiceProvider();
        }

        [Fact]
        public void ScanServiceRegistrations_TypeRegistered()
        {
            _services.ScanType(typeof(TypeRegistered));

            ServiceDescriptor descriptor = _services.Single(service => service.ServiceType == typeof(TypeRegistered));

            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
            Assert.Equal(nameof(TypeRegistered), descriptor.ImplementationFactory(_serviceProvider));
        }
    }
}

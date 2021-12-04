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
using System.Linq;
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
            _services.AddSingleton<IComparable>(_ => "Default");
            _serviceProvider = _services.BuildServiceProvider();
        }

        [Fact]
        public void ScanServiceRegistrations_TypeRegistered()
        {
            _services.ScanType(typeof(TypeRegistered), ServiceDescriptorMergeStrategy.Throw);

            Assert.Equal(3, _services.Count);
            ServiceDescriptor descriptor = _services[2];
            Assert.Equal(typeof(TypeRegistered), descriptor.ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
            Assert.Equal(nameof(TypeRegistered), descriptor.ImplementationFactory(_serviceProvider));
        }

        [Fact]
        public void ScanServiceRegistrations_FactoryRegistered()
        {
            _services.ScanType(typeof(FactoryRegistered), ServiceDescriptorMergeStrategy.Throw);

            Assert.Equal(3, _services.Count);
            ServiceDescriptor descriptor = _services[2];
            Assert.Equal(typeof(string), descriptor.ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
            Assert.Equal(nameof(FactoryRegistered.Factory1), descriptor.ImplementationFactory(_serviceProvider));
        }

        [Fact]
        public void ScanServiceRegistrations_MergeReplace()
        {
            _services.ScanType(typeof(Merge), ServiceDescriptorMergeStrategy.Replace);

            Assert.Equal(2, _services.Count);
            ServiceDescriptor descriptor = _services[1];
            Assert.Equal(typeof(IComparable), descriptor.ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
            Assert.Equal(nameof(Merge), descriptor.ImplementationFactory(_serviceProvider));
        }

        [Fact]
        public void ScanServiceRegistrations_MergeSkip()
        {
            _services.ScanType(typeof(Merge), ServiceDescriptorMergeStrategy.Skip);

            Assert.Equal(2, _services.Count);
            ServiceDescriptor descriptor = _services[1];
            Assert.Equal(typeof(IComparable), descriptor.ServiceType);
            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal("Default", descriptor.ImplementationFactory(_serviceProvider));
        }

        [Fact]
        public void ScanServiceRegistrations_MergeThrow()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                _services.ScanType(typeof(Merge), ServiceDescriptorMergeStrategy.Throw));

            Assert.Equal($"The service of type System.IComparable has already been added.", exception.Message);

            Assert.Equal(2, _services.Count);
            ServiceDescriptor descriptor = _services[1];
            Assert.Equal(typeof(IComparable), descriptor.ServiceType);
            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal("Default", descriptor.ImplementationFactory(_serviceProvider));
        }

        [Fact]
        public void ScanCurrentAssembly_Success()
        {
            _services.ScanCurrentAssembly();

            Assert.True(_services.Count > 2);
            ServiceDescriptor descriptor = _services.First(service => service.ServiceType == typeof(TypeRegistered));
            Assert.Equal(typeof(TypeRegistered), descriptor.ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
            Assert.Equal(nameof(TypeRegistered), descriptor.ImplementationFactory(_serviceProvider));
        }
    }
}

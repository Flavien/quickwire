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
using Microsoft.Extensions.DependencyInjection;
using Quickwire.Tests.Subjects;
using Xunit;

namespace Quickwire.Tests
{
    public class ServiceScannerTests
    {
        private readonly ServiceCollection _services;
        private readonly IServiceProvider _serviceProvider;

        public ServiceScannerTests()
        {
            _services = new ServiceCollection();
            _services.AddSingleton<IServiceActivator>(new MockServiceActivator());
            _serviceProvider = _services.BuildServiceProvider();
        }

        #region ScanServiceRegistrations

        [Fact]
        public void ScanServiceRegistrations_TypeRegistered()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanServiceRegistrations(typeof(TestTypes.TypeRegistered), _serviceProvider)
                .ToList();

            Assert.Single(result);
            Assert.Equal(typeof(TestTypes.TypeRegistered), result[0].ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, result[0].Lifetime);
            Assert.Equal(nameof(TestTypes.TypeRegistered), result[0].ImplementationFactory(_serviceProvider));
        }

        [Fact]
        public void ScanServiceRegistrations_TypeNotRegistered()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanServiceRegistrations(typeof(TestTypes.TypeNotRegistered), _serviceProvider)
                .ToList();

            Assert.Empty(result);
        }

        [Fact]
        public void ScanServiceRegistrations_SpecifyServiceType()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanServiceRegistrations(typeof(TestTypes.SpecifyServiceType), _serviceProvider)
                .ToList();

            Assert.Single(result);
            Assert.Equal(typeof(IDisposable), result[0].ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, result[0].Lifetime);
            Assert.Equal(nameof(TestTypes.SpecifyServiceType), result[0].ImplementationFactory(_serviceProvider));
        }

        [Fact]
        public void ScanServiceRegistrations_CanScan()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanServiceRegistrations(typeof(TestTypes.CanScan), _serviceProvider)
                .ToList();

            Assert.Single(result);
        }

        [Fact]
        public void ScanServiceRegistrations_CannotScan()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanServiceRegistrations(typeof(TestTypes.CannotScan), _serviceProvider)
                .ToList();

            Assert.Empty(result);
        }

        [Fact]
        public void ScanServiceRegistrations_StaticType()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanServiceRegistrations(typeof(TestTypes.StaticType), _serviceProvider)
                .ToList();

            Assert.Empty(result);
        }

        #endregion

        #region ScanFactoryRegistrations

        [Fact]
        public void ScanFactoryRegistrations_FactoryRegistered()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanFactoryRegistrations(typeof(TestTypes.FactoryRegistered), _serviceProvider)
                .ToList();

            Assert.Single(result);
            Assert.Equal(typeof(string), result[0].ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, result[0].Lifetime);
            Assert.Equal(nameof(TestTypes.FactoryRegistered.Factory1), result[0].ImplementationFactory(_serviceProvider));
        }

        [Fact]
        public void ScanFactoryRegistrations_FactoryNotRegistered()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanFactoryRegistrations(typeof(TestTypes.FactoryNotRegistered), _serviceProvider)
                .ToList();

            Assert.Empty(result);
        }

        [Fact]
        public void ScanFactoryRegistrations_MultipleFactoriesRegistered()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanFactoryRegistrations(typeof(TestTypes.MultipleFactoriesRegistered), _serviceProvider)
                .ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(typeof(string), result[0].ServiceType);
            Assert.Equal(ServiceLifetime.Singleton, result[0].Lifetime);
            Assert.Equal(nameof(TestTypes.MultipleFactoriesRegistered.Factory1), result[0].ImplementationFactory(_serviceProvider));
            Assert.Equal(typeof(string), result[1].ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, result[1].Lifetime);
            Assert.Equal(nameof(TestTypes.MultipleFactoriesRegistered.Factory2), result[1].ImplementationFactory(_serviceProvider));
        }

        [Fact]
        public void ScanFactoryRegistrations_SpecifyFactoryType()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanFactoryRegistrations(typeof(TestTypes.SpecifyFactoryType), _serviceProvider)
                .ToList();

            Assert.Single(result);
            Assert.Equal(typeof(IDisposable), result[0].ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, result[0].Lifetime);
            Assert.Equal(nameof(TestTypes.FactoryRegistered.Factory1), result[0].ImplementationFactory(_serviceProvider));
        }

        [Fact]
        public void ScanFactoryRegistrations_CannotScanFactory()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanFactoryRegistrations(typeof(TestTypes.CannotScanFactory), _serviceProvider)
                .ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(typeof(string), result[0].ServiceType);
            Assert.Equal(ServiceLifetime.Singleton, result[0].Lifetime);
            Assert.Equal(nameof(TestTypes.CannotScanFactory.Factory1), result[0].ImplementationFactory(_serviceProvider));
            Assert.Equal(typeof(string), result[1].ServiceType);
            Assert.Equal(ServiceLifetime.Transient, result[1].Lifetime);
            Assert.Equal(nameof(TestTypes.CannotScanFactory.Factory3), result[1].ImplementationFactory(_serviceProvider));
        }

        [Fact]
        public void ScanFactoryRegistrations_CanScanClass()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanFactoryRegistrations(typeof(TestTypes.CanScanClass), _serviceProvider)
                .ToList();

            Assert.Single(result);
            Assert.Equal(typeof(string), result[0].ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, result[0].Lifetime);
            Assert.Equal(nameof(TestTypes.CanScanClass.Factory1), result[0].ImplementationFactory(_serviceProvider));
        }

        [Fact]
        public void ScanFactoryRegistrations_CannotScanClass()
        {
            List<ServiceDescriptor> result = ServiceScanner
                .ScanFactoryRegistrations(typeof(TestTypes.CannotScanClass), _serviceProvider)
                .ToList();

            Assert.Empty(result);
        }

        #endregion
    }
}

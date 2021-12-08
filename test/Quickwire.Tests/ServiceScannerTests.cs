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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quickwire.Tests.Implementations;
using Xunit;

public partial class ServiceScannerTests
{
    private readonly ServiceCollection _services;
    private readonly IServiceProvider _serviceProvider;

    public ServiceScannerTests()
    {
        _services = new ServiceCollection();
        _services.AddSingleton<IServiceActivator>(new MockServiceActivator());
        _services.AddSingleton<IHostEnvironment>(new MockHostEnvironment());
        _serviceProvider = _services.BuildServiceProvider();
    }

    #region ScanServiceRegistrations

    [Fact]
    public void ScanServiceRegistrations_TypeRegistered()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanServiceRegistrations(typeof(TypeRegistered), _serviceProvider)
            .ToList();

        Assert.Single(result);
        Assert.Equal(typeof(TypeRegistered), result[0].ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, result[0].Lifetime);
        Assert.Equal(nameof(TypeRegistered), result[0].ImplementationFactory(_serviceProvider));
    }

    [Fact]
    public void ScanServiceRegistrations_TypeNotRegistered()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanServiceRegistrations(typeof(TypeNotRegistered), _serviceProvider)
            .ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void ScanServiceRegistrations_SpecifyServiceType()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanServiceRegistrations(typeof(SpecifyServiceType), _serviceProvider)
            .ToList();

        Assert.Single(result);
        Assert.Equal(typeof(IComparable), result[0].ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, result[0].Lifetime);
        Assert.Equal(nameof(SpecifyServiceType), result[0].ImplementationFactory(_serviceProvider));
    }

    [Fact]
    public void ScanServiceRegistrations_InvalidServiceType()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            ServiceScanner.ScanServiceRegistrations(typeof(InvalidServiceType), _serviceProvider).ToList());

        Assert.Equal(
            "The concrete type Quickwire.Tests.ServiceScannerTests+InvalidServiceType cannot be used to register service type System.IComparable.",
            exception.Message);
    }

    [Fact]
    public void ScanServiceRegistrations_CanScan()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanServiceRegistrations(typeof(CanScan), _serviceProvider)
            .ToList();

        Assert.Single(result);
    }

    [Fact]
    public void ScanServiceRegistrations_CannotScan()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanServiceRegistrations(typeof(CannotScan), _serviceProvider)
            .ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void ScanServiceRegistrations_StaticType()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanServiceRegistrations(typeof(StaticType), _serviceProvider)
            .ToList();

        Assert.Empty(result);
    }

    #endregion

    #region ScanFactoryRegistrations

    [Fact]
    public void ScanFactoryRegistrations_FactoryRegistered()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanFactoryRegistrations(typeof(FactoryRegistered), _serviceProvider)
            .ToList();

        Assert.Single(result);
        Assert.Equal(typeof(string), result[0].ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, result[0].Lifetime);
        Assert.Equal(nameof(FactoryRegistered.Factory1), result[0].ImplementationFactory(_serviceProvider));
    }

    [Fact]
    public void ScanFactoryRegistrations_FactoryNotRegistered()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanFactoryRegistrations(typeof(FactoryNotRegistered), _serviceProvider)
            .ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void ScanFactoryRegistrations_MultipleFactoriesRegistered()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanFactoryRegistrations(typeof(MultipleFactoriesRegistered), _serviceProvider)
            .ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(typeof(string), result[0].ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, result[0].Lifetime);
        Assert.Equal(nameof(MultipleFactoriesRegistered.Factory1), result[0].ImplementationFactory(_serviceProvider));
        Assert.Equal(typeof(string), result[1].ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, result[1].Lifetime);
        Assert.Equal(nameof(MultipleFactoriesRegistered.Factory2), result[1].ImplementationFactory(_serviceProvider));
    }

    [Fact]
    public void ScanFactoryRegistrations_SpecifyFactoryType()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanFactoryRegistrations(typeof(SpecifyFactoryType), _serviceProvider)
            .ToList();

        Assert.Single(result);
        Assert.Equal(typeof(IComparable), result[0].ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, result[0].Lifetime);
        Assert.Equal(nameof(FactoryRegistered.Factory1), result[0].ImplementationFactory(_serviceProvider));
    }

    [Fact]
    public void ScanFactoryRegistrations_InvalidFactoryType()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            ServiceScanner.ScanFactoryRegistrations(typeof(InvalidFactoryType), _serviceProvider).ToList());

        Assert.Equal(
            "The method Factory1 with return type System.String cannot be used to register service type System.IDisposable.",
            exception.Message);
    }

    [Fact]
    public void ScanFactoryRegistrations_CannotScanFactory()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanFactoryRegistrations(typeof(CannotScanFactory), _serviceProvider)
            .ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(typeof(string), result[0].ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, result[0].Lifetime);
        Assert.Equal(nameof(CannotScanFactory.Factory1), result[0].ImplementationFactory(_serviceProvider));
        Assert.Equal(typeof(string), result[1].ServiceType);
        Assert.Equal(ServiceLifetime.Transient, result[1].Lifetime);
        Assert.Equal(nameof(CannotScanFactory.Factory3), result[1].ImplementationFactory(_serviceProvider));
    }

    [Fact]
    public void ScanFactoryRegistrations_CanScanClass()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanFactoryRegistrations(typeof(CanScanClass), _serviceProvider)
            .ToList();

        Assert.Single(result);
        Assert.Equal(typeof(string), result[0].ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, result[0].Lifetime);
        Assert.Equal(nameof(CanScanClass.Factory1), result[0].ImplementationFactory(_serviceProvider));
    }

    [Fact]
    public void ScanFactoryRegistrations_CannotScanClass()
    {
        List<ServiceDescriptor> result = ServiceScanner
            .ScanFactoryRegistrations(typeof(CannotScanClass), _serviceProvider)
            .ToList();

        Assert.Empty(result);
    }

    #endregion
}

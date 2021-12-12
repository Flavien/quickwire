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
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quickwire.Tests.Implementations;
using Xunit;

public partial class ServiceActivatorTests
{
    private readonly ServiceActivator _activator = new ServiceActivator();
    private readonly ServiceCollection _services;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dependency _dependency = new Dependency("Default");

    public ServiceActivatorTests()
    {
        _services = new ServiceCollection();
        _services.AddSingleton<Dependency>(_dependency);
        _services.AddSingleton<string>("Test");
        _serviceProvider = _services.BuildServiceProvider();
    }

    #region GetFactory(Type)

    [Fact]
    public void GetFactoryType_ConstructorInjection()
    {
        object resultObject = _activator.GetFactory(typeof(ConstructorInjection))(_serviceProvider);
        ConstructorInjection result = resultObject as ConstructorInjection;

        Assert.NotNull(result);
        Assert.Equal(_dependency, result.Dependency1);
        Assert.Equal("Test", result.Dependency2);
    }

    [Fact]
    public void GetFactoryType_MultipleConstructors()
    {
        object resultObject = _activator.GetFactory(typeof(MultipleConstructors))(_serviceProvider);
        MultipleConstructors result = resultObject as MultipleConstructors;

        Assert.NotNull(result);
        Assert.Equal(_dependency, result.Dependency1);
        Assert.Equal("Second Constructor", result.Dependency2);
    }

    [Fact]
    public void GetFactoryType_PrivateConstructor()
    {
        object resultObject = _activator.GetFactory(typeof(PrivateConstructor))(_serviceProvider);
        PrivateConstructor result = resultObject as PrivateConstructor;

        Assert.NotNull(result);
        Assert.Equal(_dependency, result.Dependency);
    }

    [Fact]
    public void GetFactoryType_PrivateConstructorWithoutSelector()
    {
        Assert.Throws<ArgumentException>(() =>
            _activator.GetFactory(typeof(PrivateConstructorWithoutSelector)));
    }

    [Fact]
    public void GetFactoryType_NoConstructorSelector()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            _activator.GetFactory(typeof(NoConstructorSelector)));

        Assert.Equal(
            "The type Quickwire.Tests.ServiceActivatorTests+NoConstructorSelector must have exactly one public " +
            "constructor.",
            exception.Message);
    }

    [Fact]
    public void GetFactoryType_MoreThanOneConstructorSelector()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            _activator.GetFactory(typeof(MoreThanOneConstructorSelector)));

        Assert.Equal(
            "The type Quickwire.Tests.ServiceActivatorTests+MoreThanOneConstructorSelector has more than one " +
            "constructor decorated with the [ServiceConstructor] attribute.",
            exception.Message);
    }

    [Fact]
    public void GetFactoryType_ConstructorCustomInjection()
    {
        object resultObject = _activator.GetFactory(typeof(ConstructorCustomInjection))(_serviceProvider);
        ConstructorCustomInjection result = resultObject as ConstructorCustomInjection;

        Assert.NotNull(result);
        Assert.Equal("Custom Dependency", result.Dependency.Value);
    }

    [Fact]
    public void GetFactoryType_UnresolvableConstructorInjection()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _activator.GetFactory(typeof(UnresolvableConstructorInjection))(_serviceProvider));
    }

    [Fact]
    public void GetFactoryType_NoSetterInjection()
    {
        object resultObject = _activator.GetFactory(typeof(NoSetterInjection))(_serviceProvider);
        NoSetterInjection result = resultObject as NoSetterInjection;

        Assert.NotNull(result);
        Assert.Null(result.DependencyGet);
        Assert.Null(result.DependencyGetSet);
        Assert.Null(result.DependencyGetInit);
    }

    [Fact]
    public void GetFactoryType_SetterCustomInjection()
    {
        object resultObject = _activator.GetFactory(typeof(SetterCustomInjection))(_serviceProvider);
        SetterCustomInjection result = resultObject as SetterCustomInjection;

        Assert.NotNull(result);
        Assert.Equal("Custom Dependency", result.DependencyGetSet.Value);
    }

    [Fact]
    public void GetFactoryType_InheritedSetterCustomInjection()
    {
        object resultObject = _activator.GetFactory(typeof(InheritedSetterCustomInjection))(_serviceProvider);
        InheritedSetterCustomInjection result = resultObject as InheritedSetterCustomInjection;

        Assert.NotNull(result);
        Assert.Equal("Custom Dependency", result.DependencyGetSet.Value);
    }

    [Fact]
    public void GetFactoryType_NonPublicSetterCustomInjection()
    {
        object resultObject = _activator.GetFactory(typeof(NonPublicSetterCustomInjection))(_serviceProvider);
        NonPublicSetterCustomInjection result = resultObject as NonPublicSetterCustomInjection;

        Assert.NotNull(result);
        Assert.Equal("Custom Dependency 1", result.GetDependencyGetSet1().Value);
        Assert.Equal("Custom Dependency 2", result.DependencyGetSet2.Value);
        Assert.Equal("Custom Dependency 3", result.DependencyGetSet3.Value);
        Assert.Equal("Custom Dependency 4", result.DependencyGetSet4.Value);
    }

    [Fact]
    public void GetFactoryType_InitOnlySetterInjection()
    {
        object resultObject = _activator.GetFactory(typeof(InitOnlySetterInjection))(_serviceProvider);
        InitOnlySetterInjection result = resultObject as InitOnlySetterInjection;

        Assert.NotNull(result);
        Assert.Null(result.DependencyGet);
        Assert.Null(result.DependencyGetSet);
        Assert.Equal(_dependency, result.DependencyGetInit);
    }

    [Fact]
    public void GetFactoryType_InheritedInitOnlySetterInjection()
    {
        object resultObject = _activator.GetFactory(typeof(InheritedInitOnlySetterInjection))(_serviceProvider);
        InheritedInitOnlySetterInjection result = resultObject as InheritedInitOnlySetterInjection;

        Assert.NotNull(result);
        Assert.Null(result.DependencyGet);
        Assert.Null(result.DependencyGetSet);
        Assert.Equal(_dependency, result.DependencyGetInit);
    }

    [Fact]
    public void GetFactoryType_InitOnlySetterCustomInjection()
    {
        object resultObject = _activator.GetFactory(typeof(InitOnlySetterCustomInjection))(_serviceProvider);
        InitOnlySetterCustomInjection result = resultObject as InitOnlySetterCustomInjection;

        Assert.NotNull(result);
        Assert.Equal("Custom Dependency", result.DependencyGetInit.Value);
    }

    [Fact]
    public void GetFactoryType_UnresolvableInitOnlySetterInjection()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _activator.GetFactory(typeof(UnresolvableInitOnlySetterInjection))(_serviceProvider));
    }

    [Fact]
    public void GetFactoryType_StaticType()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            _activator.GetFactory(typeof(StaticType)));

        Assert.Equal(
            "The type Quickwire.Tests.ServiceActivatorTests+StaticType must have exactly one public constructor.",
            exception.Message);
    }

    #endregion

    #region GetFactory(MethodInfo)

    [Fact]
    public void GetFactoryMethodInfo_ParameterInjection()
    {
        object resultObject = _activator.GetFactory(GetMethod(nameof(Methods.ParameterInjection)))(_serviceProvider);
        string result = resultObject as string;

        Assert.Equal("Default", result);
    }

    [Fact]
    public void GetFactoryMethodInfo_ParameterCustomInjection()
    {
        object resultObject = _activator.GetFactory(GetMethod(nameof(Methods.ParameterCustomInjection)))(_serviceProvider);
        string result = resultObject as string;

        Assert.Equal("Custom Dependency", result);
    }

    [Fact]
    public void GetFactoryMethodInfo_InstanceMethod()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _activator.GetFactory(GetMethod(nameof(Methods.InstanceMethod))));
    }

    [Fact]
    public void GetFactoryMethodInfo_InternalMethod()
    {
        object resultObject = _activator.GetFactory(GetMethod(nameof(Methods.InternalMethod)))(_serviceProvider);
        string result = resultObject as string;

        Assert.Equal("Default", result);
    }

    [Fact]
    public void GetFactoryMethodInfo_PrivateMethod()
    {
        object resultObject = _activator.GetFactory(GetMethod("PrivateMethod"))(_serviceProvider);
        string result = resultObject as string;

        Assert.Equal("Default", result);
    }

    [Fact]
    public void GetFactoryMethodInfo_UnresolvableParameterInjection()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _activator.GetFactory(GetMethod(nameof(Methods.UnresolvableParameterInjection)))(_serviceProvider));
    }

    private MethodInfo GetMethod(string name) =>
        typeof(Methods).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

    #endregion
}

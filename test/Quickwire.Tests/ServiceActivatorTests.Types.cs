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
using Quickwire.Attributes;
using Quickwire.Tests.Implementations;

public partial class ServiceActivatorTests
{
    public record ConstructorInjection(Dependency Dependency1, string Dependency2);

    public class MultipleConstructors
    {
        public MultipleConstructors(Dependency dependency1, string dependency2)
        {
            Dependency1 = dependency1;
            Dependency2 = dependency2;
        }

        [ServiceConstructor]
        public MultipleConstructors(Dependency dependency1)
        {
            Dependency1 = dependency1;
            Dependency2 = "Second Constructor";
        }

        public Dependency Dependency1 { get; }

        public string Dependency2 { get; }
    }

    public class PrivateConstructor
    {
        [ServiceConstructor]
        private PrivateConstructor(Dependency dependency)
        {
            Dependency = dependency;
        }

        public Dependency Dependency { get; }
    }

    public class PrivateConstructorWithoutSelector
    {
        private PrivateConstructorWithoutSelector()
        { }
    }

    public class NoConstructorSelector
    {
        public NoConstructorSelector()
        { }

        public NoConstructorSelector(Dependency dependency)
        { }
    }

    public class MoreThanOneConstructorSelector
    {
        [ServiceConstructor]
        public MoreThanOneConstructorSelector()
        { }

        [ServiceConstructor]
        public MoreThanOneConstructorSelector(Dependency dependency)
        { }
    }

    public class ConstructorCustomInjection
    {
        public ConstructorCustomInjection(
            [TestDependencyResolver(Value = "Custom Dependency")] Dependency dependency)
        {
            Dependency = dependency;
        }

        public Dependency Dependency { get; }
    }

    public class UnresolvableConstructorInjection
    {
        public UnresolvableConstructorInjection(StringComparer dependency)
        { }
    }

    public class NoSetterInjection
    {
        public Dependency DependencyGet { get; }

        public Dependency DependencyGetSet { get; set; }

        public Dependency DependencyGetInit { get; init; }
    }

    public class SetterCustomInjection
    {
        [TestDependencyResolver(Value = "Custom Dependency")]
        public Dependency DependencyGetSet { get; set; }
    }

    public class InheritedSetterCustomInjection : SetterCustomInjection
    { }

    public class NonPublicSetterCustomInjection
    {
        [TestDependencyResolver(Value = "Custom Dependency 1")]
        private Dependency DependencyGetSet1 { get; set; }

        [TestDependencyResolver(Value = "Custom Dependency 2")]
        public Dependency DependencyGetSet2 { get; private set; }

        [TestDependencyResolver(Value = "Custom Dependency 3")]
        public Dependency DependencyGetSet3 { get; protected set; }

        [TestDependencyResolver(Value = "Custom Dependency 4")]
        public Dependency DependencyGetSet4 { get; internal set; }

        public Dependency GetDependencyGetSet1() => DependencyGetSet1;
    }

    [InjectAllInitOnlyProperties]
    public class InitOnlySetterInjection
    {
        public Dependency DependencyGet { get; }

        public Dependency DependencyGetSet { get; set; }

        public Dependency DependencyGetInit { get; init; }
    }

    public class InheritedInitOnlySetterInjection : InitOnlySetterInjection
    { }

    [InjectAllInitOnlyProperties]
    public class InitOnlySetterCustomInjection
    {
        [TestDependencyResolver(Value = "Custom Dependency")]
        public Dependency DependencyGetInit { get; init; }
    }

    [InjectAllInitOnlyProperties]
    public class UnresolvableInitOnlySetterInjection
    {
        public StringComparer DependencyGetInit { get; init; }
    }

    public static class StaticType
    {
        static StaticType() { }
    }

    public static class GenericType<T>
    { }

    public class Methods
    {
        public static string ParameterInjection(Dependency dependency) => dependency.Value;

        public static string ParameterCustomInjection(
            [TestDependencyResolver(Value = "Custom Dependency")] Dependency dependency)
        {
            return dependency.Value;
        }

        internal static string InternalMethod(Dependency dependency) => dependency.Value;

        private static string PrivateMethod(Dependency dependency) => dependency.Value;

        public static string UnresolvableParameterInjection(StringComparer dependency) => "";

        public string InstanceMethod(Dependency dependency) => dependency.Value;

        public static string GenericMethod<T>(Dependency dependency) => dependency.Value;

        public class GenericType<T>
        {
            public static string Method(Dependency dependency) => dependency.Value;
        }
    }
}

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
using Quickwire.Attributes;

namespace Quickwire.Tests.Subjects
{
    public static class TestObjects
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

        public class SetterInjection
        {
            [InjectService]
            public Dependency DependencyGetSet { get; set; }
        }

        public class CustomSetterInjection
        {
            [TestDependencyResolver(Value = "Custom Dependency")]
            public Dependency DependencyGetSet { get; set; }
        }

        [InjectAllInitOnlyProperties]
        public class InitOnlySetterInjection
        {
            public Dependency DependencyGet { get; }

            public Dependency DependencyGetSet { get; set; }

            public Dependency DependencyGetInit { get; init; }
        }

        [InjectAllInitOnlyProperties]
        public class InitOnlyCustomSetterInjection
        {
            public Dependency DependencyGetInit1 { get; init; }

            [InjectService]
            public Dependency DependencyGetInit2 { get; init; }

            [TestDependencyResolver(Value = "Custom Dependency")]
            public Dependency DependencyGetInit3 { get; init; }
        }

        public class NonPublicSetter
        {
            [InjectService]
            private Dependency DependencyGetSet1 { get; set; }

            [InjectService]
            public Dependency DependencyGetSet2 { get; private set; }

            [InjectService]
            public Dependency DependencyGetSet3 { get; protected set; }

            [InjectService]
            public Dependency DependencyGetSet4 { get; internal set; }

            public Dependency GetDependencyGetSet1() => DependencyGetSet1;
        }

        public class UnresolvableSetterInjection
        {
            [InjectService]
            public StringComparer DependencyGetSet { get; set; }
        }

        public class OptionalSetterInjection
        {
            [InjectService(Optional = true)]
            public StringComparer DependencyGetSet { get; set; }
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
    }
}

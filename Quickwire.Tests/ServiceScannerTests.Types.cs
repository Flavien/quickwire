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
using Quickwire.Tests.TestImplementations;

namespace Quickwire.Tests
{
    public partial class ServiceScannerTests
    {
        [RegisterService(ServiceLifetime.Scoped)]
        public class TypeRegistered { }

        public class TypeNotRegistered { }

        [RegisterService(ServiceLifetime.Scoped, ServiceType = typeof(IDisposable))]
        public class SpecifyServiceType { }

        [TestServiceScanningFilter(Active = true)]
        [RegisterService(ServiceLifetime.Scoped)]
        public class CanScan { }

        [TestServiceScanningFilter(Active = false)]
        [RegisterService(ServiceLifetime.Scoped)]
        public class CannotScan { }

        [RegisterService(ServiceLifetime.Scoped)]
        public static class StaticType { }

        public class FactoryRegistered
        {
            [RegisterFactory(ServiceLifetime.Scoped)]
            public static string Factory1() => "";
        }

        public class FactoryNotRegistered
        {
            public static string Factory1() => "";
        }

        public class MultipleFactoriesRegistered
        {
            [RegisterFactory(ServiceLifetime.Singleton)]
            public static string Factory1() => "";

            [RegisterFactory(ServiceLifetime.Scoped)]
            public static string Factory2() => "";
        }

        public class SpecifyFactoryType
        {
            [RegisterFactory(ServiceLifetime.Scoped, ServiceType = typeof(IDisposable))]
            public static string Factory1() => "";
        }

        public class CannotScanFactory
        {
            [RegisterFactory(ServiceLifetime.Singleton)]
            public static string Factory1() => "";

            [RegisterFactory(ServiceLifetime.Scoped)]
            [TestServiceScanningFilter(Active = false)]
            public static string Factory2() => "";

            [RegisterFactory(ServiceLifetime.Transient)]
            [TestServiceScanningFilter(Active = true)]
            public static string Factory3() => "";
        }

        [TestServiceScanningFilter(Active = true)]
        public class CanScanClass
        {
            [RegisterFactory(ServiceLifetime.Scoped)]
            public static string Factory1() => "";
        }

        [TestServiceScanningFilter(Active = false)]
        public class CannotScanClass
        {
            [RegisterFactory(ServiceLifetime.Scoped)]
            public static string Factory1() => "";
        }
    }
}

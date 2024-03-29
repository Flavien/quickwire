﻿// Copyright 2021 Flavien Charlon
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
using Microsoft.Extensions.DependencyInjection;
using Quickwire.Attributes;

public partial class ServiceCollectionExtensionsTests
{
    [RegisterService(ServiceLifetime.Scoped)]
    public class TypeRegistered { }

    public class FactoryRegistered
    {
        [RegisterFactory(ServiceLifetime.Scoped)]
        public static string Factory1() => "";
    }

    [RegisterService(ServiceLifetime.Scoped, ServiceType = typeof(IComparable))]
    public class Merge : IComparable
    {
        public int CompareTo(object obj) => throw new NotImplementedException();
    }
}

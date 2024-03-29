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

namespace Quickwire.Tests.Implementations;

using System;
using System.Reflection;

public class MockServiceActivator : IServiceActivator
{
    public Func<IServiceProvider, object> GetFactory(MethodInfo methodInfo)
    {
        return _ => methodInfo.Name;
    }

    public Func<IServiceProvider, object> GetFactory(Type type)
    {
        return _ => type.Name;
    }
}

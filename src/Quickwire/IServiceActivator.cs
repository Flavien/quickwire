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

namespace Quickwire;

using System;
using System.Reflection;

/// <summary>
/// Represents a class that can construct factory methods based either on type constructors or static methods.
/// </summary>
public interface IServiceActivator
{
    /// <summary>
    /// Returns a factory method based on a static method.
    /// </summary>
    Func<IServiceProvider, object?> GetFactory(MethodInfo methodInfo);

    /// <summary>
    /// Returns a factory method based on a type constructor.
    /// </summary>
    Func<IServiceProvider, object> GetFactory(Type type);
}

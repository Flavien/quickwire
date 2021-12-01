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

namespace Quickwire.Tests.Subjects
{
    public class TestMethods
    {
        public static string ParameterInjection(Dependency dependency) => dependency.Value;

        public string InstanceMethod(Dependency dependency) => dependency.Value;

        internal static string InternalMethod(Dependency dependency) => dependency.Value;

        private static string PrivateMethod(Dependency dependency) => dependency.Value;

        public static string UnresolvableParameterInjection(StringComparer dependency) => "";
    }
}

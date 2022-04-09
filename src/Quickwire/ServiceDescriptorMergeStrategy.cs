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

public enum ServiceDescriptorMergeStrategy
{
    /// <summary>
    /// Ignore conflicts and add conflicting service descriptors to the collection.
    /// </summary>
    Add,
    /// <summary>
    /// Remove the first service descriptors with the same service type in case of conflict, then add the new
    /// service descriptor to the collection.
    /// </summary>
    Replace,
    /// <summary>
    /// Add a service descriptor to the collection only if the service type hasn't already been registered.
    /// </summary>
    TryAdd,
    /// <summary>
    /// Throw an exception in case of conflicting service descriptors.
    /// </summary>
    Throw
}

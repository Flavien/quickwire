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
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quickwire.Attributes;
using Xunit;

public class InjectConfigurationAttributeTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly InjectConfigurationAttribute _injectConfiguration;

    public InjectConfigurationAttributeTests()
    {
        ServiceCollection services = new ServiceCollection();
        _configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        services.AddSingleton<IConfiguration>(_configuration);
        _serviceProvider = services.BuildServiceProvider();
        _injectConfiguration = new InjectConfigurationAttribute("key");
    }

    [Theory]
    [MemberData(nameof(ResolveData))]
    public void Resolve_Success(string configurationValue, Type type, object expected)
    {
        _configuration["key"] = configurationValue;

        object result = _injectConfiguration.Resolve(_serviceProvider, type);

        if (configurationValue != null)
            Assert.IsAssignableFrom(type, result);

        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> ResolveData =>
        new List<object[]>
        {
            new object[] { "value", typeof(string), "value" },
            new object[] { null, typeof(string), null },
            new object[] { "true", typeof(bool), true },
            new object[] { "100", typeof(int), 100 },
            new object[] { "100", typeof(long), 100L },
            new object[] { "10.01", typeof(decimal), 10.01m },
            new object[] { "10.01", typeof(double), 10.01d },
            new object[] { null, typeof(int?), (int?)null },
            new object[] { "100", typeof(int?), new Nullable<int>(100) },
            new object[] { "08:50:15.500", typeof(TimeSpan), TimeSpan.FromSeconds(15.5 + 60 * (50 + 60 * 8)) },
            new object[] { "2020-12-25T16:08:30", typeof(DateTime), new DateTime(2020, 12, 25, 16, 8, 30) },
            new object[] { "2020-12-25", typeof(DateOnly), new DateOnly(2020, 12, 25) },
            new object[] { "13:14:15", typeof(TimeOnly), new TimeOnly(13, 14, 15, 0) },
            new object[] { "127.0.0.1", typeof(IPAddress), new IPAddress(new byte[] { 127, 0, 0, 1 }) },
            new object[] { "https://host/path?a=b", typeof(Uri), new Uri("https://host/path?a=b") },
            new object[] { "23688849-6394-4492-86ee-3cc2ef5a992f", typeof(Guid), Guid.Parse("23688849-6394-4492-86ee-3cc2ef5a992f") },
            new object[] { "Local", typeof(DateTimeKind), DateTimeKind.Local }
        };

    [Fact]
    public void Resolve_CannotConvert()
    {
        _configuration["key"] = "...";

        Assert.Throws<InvalidCastException>(
            () => _injectConfiguration.Resolve(_serviceProvider, typeof(StringComparer)));
    }
}

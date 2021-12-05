# Quickwire
[![Quickwire](https://img.shields.io/nuget/v/Quickwire.svg?style=flat-square&color=blue&logo=nuget)](https://www.nuget.org/packages/Quickwire/)

Attribute-based dependency injection for .NET.

## Features

- Register services for dependency injection using attributes and reduce boilerplate code.
- Uses the built-in .NET dependency injection container. No need to use a third party container or to change your exising code.
- Full support for property injection.
- Inject configuration settings semlessly into your services.
- Use configuration selectors to register different implementations based on the current environment.

## Quick start

1. Install from NuGet:

```
   nuget install Quickwire
```

2. Activate Quickwire for the current assembly in the `AddServices` method of the `Startup` class:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.ScanCurrentAssembly();
}
```

3. Decorate services with this attribute in order to register them for dependency injection:

```csharp
[RegisterService(ServiceLifetime.Scoped)]
public class MyService
{
    // ...
}
```

## Registering a service

There are two ways to register a service using Quickwire.
- Apply the `[RegisterService]` attribute to a class.
- Apply the `[RegisterFactory]` attribute to a static factory method.

## Registering a service using a class

### Registration

When applying the `[RegisterService]` attribute to a class, the class will be registered as the concrete type to use for the service of the same type.

It is also possible to specify the `ServiceType` property on the `[RegisterService]` attribute to register the concrete type under a different service type. The `ServiceType` should be parent type, or interface implemented by the class.

The attribute takes a parameter of type `ServiceLifetime` to specify the lifetime of the service.

### Instantiation

By default, Quickwire will use the public constructor to instantiate the concrete type. There must be exactly one public constructor available or an exception will be thrown.

If there are more than one public constructor, or if a non-public constructor should be used, the `[ServiceConstructor]` should be applied to indicate explicitely and unambiguously which constructor to use.

By default, all the contructor parameters will be resolved using dependency injection, however it is also possible to decorate parameters using the `[InjectConfiguration]` attribute to inject a configuration setting. It is also possible to use `[InjectService(Optional = true)]` to make a dependency optional.

### Property injection

Property injection can be achived by decorating a property with a setter with the `[InjectService]` attribute.

```csharp
[RegisterService(ServiceLifetime.Singleton)]
public class MyService
{
    [InjectService]
    public IHttpClientBuilder HttpClientBuilder { get; private set; }

    // ...
}
```

### Automatic init-only property injection

By decorating a service class with the `[InjectAllInitOnlyProperties]` attribute, dependency injection will be performed on all init-only properties in the class.

```csharp
[RegisterService(ServiceLifetime.Singleton)]
[InjectAllInitOnlyProperties]
public class MyService
{
    public IHttpClientBuilder HttpClientBuilder { get; init; }

    // ...
}
```

## Registering a service using a factory

When applying the `[RegisterFactory]` attribute to a static method, the static method will be registered as a factory used 

By default, all the method parameters will be resolved using dependency injection, however it is also possible to decorate parameters using the `[InjectConfiguration]` attribute to inject a configuration setting. It is also possible to use `[InjectService(Optional = true)]` to make a dependency optional.

## Configuration setting injection

Constructor parameters, properties and factory parameters can also be decorated with the `[InjectConfiguration]` attribute in order to inject a configuration setting.

Conversion to most basic types, including enumeration types, is supported.

```csharp
[RegisterService(ServiceLifetime.Scoped)]
public class MyService
{
    [InjectConfiguration("external_api:url")]
    public string ExternalApiUrl { get; init; }

    [InjectConfiguration("external_api:retries")]
    public int Retries { get; init; }

    [InjectConfiguration("external_api:timeout")]
    public TimeSpan Timeout { get; init; }

    // ...
}
```

## Environment selection

It is possible to disable specific services or service factories using the `[EnvironmentSelector]` attribute.

```csharp
[EnvironmentSelector(Enabled = new[] { "Development" })]
public class DebugFactories
{
    // This is only registered in the Development environment
    [RegisterFactory(ServiceLifetime.Transient)]
    public ILogger CreateDebugLogger()
    {
        // ...
    }

    // ...
}
```

## Equivalence with Spring Framework in Java

The approach provided by Quickwire in .NET matches closely the approach that can be used in Java with the Spring Framework. The table below outlines the similarities between both approaches.

|       | Quickwire | Spring |
| ----- | --------- | ------ |
| Register a class for dependency injection | `[RegisterService]` | `@Service`, `@Component`, `@Repository` |
| Register a factory method | `[RegisterFactory]` | `@Bean` |
| Property injection | `[InjectService]` | `@Autowired` |
| Configuration setting injection | `[InjectConfiguration]` | `@Value` |
| Selective activation based on environment | `[EnvironmentSelector]` | `@Profile("profile")`
| Bootstrap | `services.ScanCurrentAssembly()` | `@EnableAutoConfiguration`, `@ComponentScan` |
## License

Copyright 2021 Flavien Charlon

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and limitations under the License.

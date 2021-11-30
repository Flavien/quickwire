using System;
using Microsoft.Extensions.DependencyInjection;

namespace Quickwire.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RegisterFactoryAttribute : Attribute
    {
        public RegisterFactoryAttribute(ServiceLifetime scope)
        {
            Scope = scope;
        }

        public ServiceLifetime Scope { get; set; }

        public Type? ServiceType { get; set; }
    }
}

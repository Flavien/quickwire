using Microsoft.Extensions.DependencyInjection;
using System;

namespace SpringOnion.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class RegisterServiceAttribute : Attribute
    {
        public RegisterServiceAttribute(ServiceLifetime scope)
        {
            Scope = scope;
        }

        public ServiceLifetime Scope { get; set; }

        public Type? ServiceType { get; set; }
    }
}

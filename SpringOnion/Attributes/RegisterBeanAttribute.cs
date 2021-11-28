using Microsoft.Extensions.DependencyInjection;
using System;

namespace SpringOnion.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RegisterBeanAttribute : Attribute
    {
        public RegisterBeanAttribute(ServiceLifetime scope)
        {
            Scope = scope;
        }

        public ServiceLifetime Scope { get; set; }

        public Type? ServiceType { get; set; }
    }
}

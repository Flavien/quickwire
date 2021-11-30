using Microsoft.Extensions.DependencyInjection;
using System;

namespace SpringOnion.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class DependencyResolverAttribute : Attribute
    {
        public abstract object Resolve(IServiceProvider serviceProvider, Type type);

        public static object Resolve(IServiceProvider serviceProvider, Type serviceType, DependencyResolverAttribute? dependencyResolver)
        {
            if (dependencyResolver == null)
                return serviceProvider.GetRequiredService(serviceType);
            else
                return dependencyResolver.Resolve(serviceProvider, serviceType);
        }
    }
}

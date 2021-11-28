using Microsoft.Extensions.DependencyInjection;
using System;

namespace SpringOnion.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InjectServiceAttribute : DependencyResolverAttribute
    {
        public override object Resolve(IServiceProvider serviceProvider, Type type)
        {
            return serviceProvider.GetRequiredService(type);
        }
    }
}

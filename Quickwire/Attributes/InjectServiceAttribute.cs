using System;
using Microsoft.Extensions.DependencyInjection;

namespace Quickwire.Attributes
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

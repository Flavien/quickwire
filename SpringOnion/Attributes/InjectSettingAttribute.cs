using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SpringOnion.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InjectSettingAttribute : DependencyResolverAttribute
    {
        public InjectSettingAttribute(string configurationKey)
        {
            ConfigurationKey = configurationKey;
        }

        public string ConfigurationKey { get; set; }

        public override object Resolve(IServiceProvider serviceProvider, Type type)
        {
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

            return configuration[ConfigurationKey];
        }
    }
}

using System;
using System.Linq;

namespace SpringOnion.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class EnvironmentSelectorAttribute : Attribute
    {
        public string[]? Enabled { get; set; }

        public string[]? Disabled { get; set; }

        public static bool IsEnabled(EnvironmentSelectorAttribute? attribute, string environmentName)
        {
            if (attribute == null)
            {
                return true;
            }
            else
            {
                bool enabled = true;
                if (attribute.Enabled != null)
                {
                    enabled &= attribute.Enabled.Contains(environmentName, StringComparer.OrdinalIgnoreCase);
                }

                if (attribute.Disabled != null)
                {
                    enabled &= !attribute.Disabled.Contains(environmentName, StringComparer.OrdinalIgnoreCase);
                }

                return enabled;
            }
        }
    }
}

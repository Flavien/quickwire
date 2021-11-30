using System;

namespace SpringOnion.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class InjectAllInitOnlyPropertiesAttribute : Attribute
    { }
}

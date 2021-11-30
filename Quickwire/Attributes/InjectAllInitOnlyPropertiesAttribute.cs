using System;

namespace Quickwire.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class InjectAllInitOnlyPropertiesAttribute : Attribute
    { }
}

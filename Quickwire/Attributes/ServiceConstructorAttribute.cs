using System;

namespace Quickwire.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class ServiceConstructorAttribute : Attribute
    { }
}

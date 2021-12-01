using System;
using System.Reflection;

namespace Quickwire
{
    public interface IServiceActivator
    {
        Func<IServiceProvider, object?> GetFactory(MethodInfo methodInfo);

        Func<IServiceProvider, object> GetFactory(Type type);
    }
}

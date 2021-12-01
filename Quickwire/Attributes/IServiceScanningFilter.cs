using System;

namespace Quickwire.Attributes
{
    public interface IServiceScanningFilter
    {
        bool CanScan(IServiceProvider serviceProvider);
    }
}

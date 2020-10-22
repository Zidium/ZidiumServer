using System;

namespace Zidium.Core
{
    public interface IDispatcherConfiguration
    {
        bool UseLocalDispatcher { get; }

        Uri DispatcherUrl { get; }
    }
}

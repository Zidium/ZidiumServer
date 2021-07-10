using System;

namespace Zidium.Core.Tests.Caching
{
    public interface ITestCacheUserReadObject
    {
        Guid Id { get; }

        string Name { get; }

        int Money { get; }
    }
}

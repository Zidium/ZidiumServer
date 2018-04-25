using System;

namespace Zidium.Core.Caching
{
    public interface ICacheWriteObject : IDisposable
    {
        Guid Id { get; }

        long SaveOrder { get; set; }

        int DataVersion { get; }

        void IncrementDataVersion();

        int GetCacheSize();

        void BeginSave();

        void Unload();
    }
}

using System;

namespace Zidium.Storage
{
    public interface ITransaction : IDisposable
    {
        void Commit();

        void Rollback();
    }
}

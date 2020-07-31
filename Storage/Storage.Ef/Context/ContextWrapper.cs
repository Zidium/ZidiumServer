using System;

namespace Zidium.Storage.Ef
{
    internal class ContextWrapper : IDisposable
    {
        public ContextWrapper(AccountDbContext context, Action onDispose)
        {
            Context = context;
            _onDispose = onDispose;
        }

        private readonly Action _onDispose;

        public void Dispose()
        {
            _onDispose();
        }

        public AccountDbContext Context { get; }
    }
}

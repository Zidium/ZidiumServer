using System.Data.Entity;

namespace Zidium.Storage.Ef
{
    internal class Transaction : ITransaction
    {
        public Transaction(ContextWrapper contextWrapper)
        {
            _contextWrapper = contextWrapper;
            _transaction = _contextWrapper.Context.Database.BeginTransaction();
        }

        private readonly ContextWrapper _contextWrapper;

        private bool _finished;

        private readonly DbContextTransaction _transaction;

        public void Dispose()
        {
            if (!_finished)
            {
                Rollback();
            }
            _contextWrapper.Dispose();
        }

        public void Commit()
        {
            _transaction.Commit();
            _finished = true;
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _finished = true;
        }
    }
}

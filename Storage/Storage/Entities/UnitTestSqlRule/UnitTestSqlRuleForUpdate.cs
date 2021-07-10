using System;

namespace Zidium.Storage
{
    public class UnitTestSqlRuleForUpdate
    {
        public UnitTestSqlRuleForUpdate(Guid unitTestId)
        {
            UnitTestId = unitTestId;
            Provider = new ChangeTracker<SqlRuleDatabaseProviderType>();
            ConnectionString = new ChangeTracker<string>();
            OpenConnectionTimeoutMs = new ChangeTracker<int>();
            CommandTimeoutMs = new ChangeTracker<int>();
            Query = new ChangeTracker<string>();
        }

        /// <summary>
        /// Ссылка на проверку
        /// </summary>
        public Guid UnitTestId { get; }

        public ChangeTracker<SqlRuleDatabaseProviderType> Provider { get; }

        public ChangeTracker<string> ConnectionString { get; }

        public ChangeTracker<int> OpenConnectionTimeoutMs { get; }

        public ChangeTracker<int> CommandTimeoutMs { get; }

        public ChangeTracker<string> Query { get; }

    }
}
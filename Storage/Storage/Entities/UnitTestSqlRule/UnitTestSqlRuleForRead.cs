using System;

namespace Zidium.Storage
{
    public class UnitTestSqlRuleForRead
    {
        public UnitTestSqlRuleForRead(
            Guid unitTestId,
            SqlRuleDatabaseProviderType provider,
            string connectionString,
            int openConnectionTimeoutMs,
            int commandTimeoutMs,
            string query)
        {
            UnitTestId = unitTestId;
            Provider = provider;
            ConnectionString = connectionString;
            OpenConnectionTimeoutMs = openConnectionTimeoutMs;
            CommandTimeoutMs = commandTimeoutMs;
            Query = query;
        }

        /// <summary>
        /// Ссылка на проверку
        /// </summary>
        public Guid UnitTestId { get; }

        public SqlRuleDatabaseProviderType Provider { get; }

        public string ConnectionString { get; }

        public int OpenConnectionTimeoutMs { get; }

        public int CommandTimeoutMs { get; }

        public string Query { get; }

        public UnitTestSqlRuleForUpdate GetForUpdate()
        {
            return new UnitTestSqlRuleForUpdate(UnitTestId);
        }

    }
}

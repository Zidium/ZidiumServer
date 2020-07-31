using System;

namespace Zidium.Storage
{
    public class UnitTestSqlRuleForAdd
    {
        /// <summary>
        /// Ссылка на проверку
        /// </summary>
        public Guid UnitTestId;

        public SqlRuleDatabaseProviderType Provider;

        public string ConnectionString;

        public int OpenConnectionTimeoutMs;

        public int CommandTimeoutMs;

        public string Query;

    }
}

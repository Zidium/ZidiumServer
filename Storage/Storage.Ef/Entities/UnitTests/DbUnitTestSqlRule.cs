using System;

namespace Zidium.Storage.Ef
{
    public class DbUnitTestSqlRule
    {
        /// <summary>
        /// Ссылка на юнит-тест
        /// </summary>
        public Guid UnitTestId { get; set; }

        /// <summary>
        /// юнит-тест
        /// </summary>
        public virtual DbUnitTest UnitTest { get; set; }

        public SqlRuleDatabaseProviderType Provider { get; set; }

        public string ConnectionString { get; set; }

        public int OpenConnectionTimeoutMs { get; set; }

        public int CommandTimeoutMs { get; set; }

        public string Query { get; set; }

    }
}

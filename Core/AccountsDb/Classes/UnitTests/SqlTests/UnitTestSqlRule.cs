using System;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class UnitTestSqlRule
    {
        /// <summary>
        /// Ссылка на юнит-тест
        /// </summary>
        public Guid UnitTestId { get; set; }

        /// <summary>
        /// юнит-тест
        /// </summary>
        public virtual UnitTest UnitTest { get; set; }

        public DatabaseProviderType Provider { get; set; }

        public string ConnectionString { get; set; }

        public int OpenConnectionTimeoutMs { get; set; }

        public int CommandTimeoutMs { get; set; }

        public string Query { get; set; }
    }
}

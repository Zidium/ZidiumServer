using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using Zidium.Core.AccountsDb.Migrations.MsSql;
using Zidium.Core.AccountsDb.Migrations.MySql;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using Npgsql;
using Zidium.Core.AccountsDb.Migrations.PostgreSql;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Провайдер базы данных
    /// </summary>
    public class Provider
    {
        protected static Provider FCurrent;

        public static Provider Current()
        {
            if (FCurrent == null)
            {
                FCurrent = new Provider(_invariantName);
            }
            return FCurrent;
        }

        protected Provider(DatabaseProviderType databaseProviderType)
        {
            Type = databaseProviderType;
        }

        protected Provider(string invariantName)
        {
            if (invariantName.Equals("System.Data.SqlClient", StringComparison.OrdinalIgnoreCase))
                Type = DatabaseProviderType.MsSql;
            else if (invariantName.Equals("MySql.Data.MySqlClient", StringComparison.OrdinalIgnoreCase))
                Type = DatabaseProviderType.MySql;
            else if (invariantName.Equals("Npgsql", StringComparison.OrdinalIgnoreCase))
                Type = DatabaseProviderType.PostgreSql;
            else
                throw new Exception("Unsupported provider " + invariantName);
        }

        static Provider()
        {
            SetSectionName("DbContext");
        }

        public static void SetSectionName(string sectionName)
        {
            _invariantName = ConfigurationManager.ConnectionStrings[sectionName]?.ProviderName;
        }

        private static string _invariantName;

        /// <summary>
        /// Тип провайдера
        /// </summary>
        public DatabaseProviderType Type { get; protected set; }

        /// <summary>
        /// Имя провайдера
        /// </summary>
        public string InvariantName
        {
            get
            {
                if (Type == DatabaseProviderType.MsSql)
                    return "System.Data.SqlClient";
                if (Type == DatabaseProviderType.MySql)
                    return "MySql.Data.MySqlClient";
                if (Type == DatabaseProviderType.PostgreSql)
                    return "Npgsql";
                return null;
            }
        }

        /// <summary>
        /// Конфигурация провайдера
        /// </summary>
        public DbConfiguration Configuration()
        {
            if (Type == DatabaseProviderType.MsSql)
                return null;
            if (Type == DatabaseProviderType.MySql)
                return new MySqlEFConfiguration();
            if (Type == DatabaseProviderType.PostgreSql)
                return null;
            return null;
        }

        /// <summary>
        /// Объект соединения
        /// </summary>
        public DbConnection Connection(string connectionString)
        {
            if (Type == DatabaseProviderType.MsSql)
                return new SqlConnection(connectionString);
            if (Type == DatabaseProviderType.MySql)
                return new MySqlConnection(connectionString);
            if (Type == DatabaseProviderType.PostgreSql)
                return new NpgsqlConnection(connectionString);
            return null;
        }

        public AccountDbContext DbContext(string connectionString)
        {
            if (Type == DatabaseProviderType.MsSql)
                return new MsSqlAccountDbContext(Connection(connectionString));
            if (Type == DatabaseProviderType.MySql)
                return new MySqlAccountDbContext(Connection(connectionString));
            if (Type == DatabaseProviderType.PostgreSql)
                return new PostgreSqlAccountDbContext(Connection(connectionString));
            return null;
        }

        public DbMigrationsConfiguration DbMConfiguration()
        {
            if (Type == DatabaseProviderType.MsSql)
                return new MsSqlAccountDbMConfiguration();
            if (Type == DatabaseProviderType.MySql)
                return new MySqlAccountDbMConfiguration();
            if (Type == DatabaseProviderType.PostgreSql)
                return new PostgreSqlAccountDbMConfiguration();
            return null;
        }

        public int InitializeDb(string connectionString)
        {
            var conf = DbMConfiguration();
            conf.TargetDatabase = new DbConnectionInfo(connectionString, InvariantName);
            var mirgator = new DbMigrator(conf);
            var result = mirgator.GetPendingMigrations().Count();
            mirgator.Update();
            return result;
        }

        public string FormatSchemaName(string schemaName)
        {
            if (Type == DatabaseProviderType.MsSql)
                return "[" + schemaName + "]";
            if (Type == DatabaseProviderType.PostgreSql)
                return "\"" + schemaName + "\"";
            return schemaName;
        }

        public string FormatTableName(string tableName)
        {
            if (Type == DatabaseProviderType.MsSql)
                return "[" + tableName + "]";
            if (Type == DatabaseProviderType.PostgreSql)
                return "\"" + tableName + "\"";
            return tableName;
        }

        public string FormatColumnName(string columnName)
        {
            if (Type == DatabaseProviderType.MsSql)
                return "[" + columnName + "]";
            if (Type == DatabaseProviderType.PostgreSql)
                return "\"" + columnName + "\"";
            return columnName;
        }
    }
}

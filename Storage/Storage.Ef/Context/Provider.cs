using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using Npgsql;
using Zidium.Storage.Ef.Migrations.MsSql;
using Zidium.Storage.Ef.Migrations.PostgreSql;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Провайдер базы данных
    /// </summary>
    internal class Provider
    {
        protected static Provider FCurrent;

        public static Provider Current(string sectionName = null)
        {
            if (FCurrent == null)
            {
                var invariantName = ConfigurationManager.ConnectionStrings[sectionName ?? "DbContext"].ProviderName;
                FCurrent = new Provider(invariantName);
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
                if (Type == DatabaseProviderType.PostgreSql)
                    return "Npgsql";
                return null;
            }
        }

        /// <summary>
        /// Объект соединения
        /// </summary>
        public DbConnection Connection(string connectionString)
        {
            if (Type == DatabaseProviderType.MsSql)
                return new SqlConnection(connectionString);
            /*
            if (Type == DatabaseProviderType.PostgreSql)
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString)
                {
                    MaxPoolSize = 1024,
                    ConnectionIdleLifetime = 10
                };
                return new NpgsqlConnection(builder.ConnectionString);
            }
            */
            if (Type == DatabaseProviderType.PostgreSql)
                return new NpgsqlConnection(connectionString);
            return null;
        }

        public AccountDbContext DbContext(string connectionString)
        {
            if (Type == DatabaseProviderType.MsSql)
                return new MsSqlAccountDbContext(Connection(connectionString));
            if (Type == DatabaseProviderType.PostgreSql)
                return new PostgreSqlAccountDbContext(Connection(connectionString));
            return null;
        }

        public DbMigrationsConfiguration DbMConfiguration()
        {
            if (Type == DatabaseProviderType.MsSql)
                return new MsSqlAccountDbMConfiguration();
            if (Type == DatabaseProviderType.PostgreSql)
                return new PostgreSqlAccountDbMConfiguration();
            return null;
        }

        public int Migrate(string connectionString)
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

using System;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Npgsql;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Провайдер базы данных
    /// </summary>
    internal class Provider
    {
        protected static Provider FCurrent;

        public static Provider Current()
        {
            if (FCurrent == null)
            {
                var providerName = DependencyInjection.GetServicePersistent<IDatabaseConfiguration>().ProviderName;
                FCurrent = new Provider(providerName);
            }
            return FCurrent;
        }

        protected Provider(DatabaseProviderType databaseProviderType)
        {
            Type = databaseProviderType;
        }

        protected Provider(string providerName)
        {
            if (providerName.Equals("System.Data.SqlClient", StringComparison.OrdinalIgnoreCase))
                Type = DatabaseProviderType.MsSql;
            else if (providerName.Equals("Npgsql", StringComparison.OrdinalIgnoreCase))
                Type = DatabaseProviderType.PostgreSql;
            else
                throw new Exception("Unsupported provider " + providerName);
        }

        /// <summary>
        /// Тип провайдера
        /// </summary>
        public DatabaseProviderType Type { get; protected set; }

        /// <summary>
        /// Имя провайдера
        /// </summary>
        public string ProviderName
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

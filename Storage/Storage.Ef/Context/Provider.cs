using System;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using Zidium.Common;
using Zidium.Storage.Ef.Sqlite;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Провайдер базы данных
    /// </summary>
    internal class Provider
    {
        // TODO Refactor to service? Not sure it is nesessary for internal class
        public static Provider Instance()
        {
            if (_instance == null)
            {
                var databaseConfiguration = DependencyInjection.GetServicePersistent<IDatabaseConfiguration>();
                _instance = new Provider(databaseConfiguration);
            }
            return _instance;
        }

        private static Provider _instance;

        public Provider(IDatabaseConfiguration databaseConfiguration)
        {
            var providerName = databaseConfiguration.ProviderName;

            if (providerName.Equals("System.Data.SqlClient", StringComparison.OrdinalIgnoreCase))
                Type = DatabaseProviderType.MsSql;
            else if (providerName.Equals("Npgsql", StringComparison.OrdinalIgnoreCase))
                Type = DatabaseProviderType.PostgreSql;
            else if (providerName.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
                Type = DatabaseProviderType.Sqlite;
            else
                throw new Exception("Unsupported provider " + providerName);

            _connectionString = databaseConfiguration.ConnectionString;

            if (Type == DatabaseProviderType.Sqlite)
                _connectionString = SqliteConnectionStringHelper.SubstituteEnvVariables(_connectionString);
        }

        private readonly string _connectionString;

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
                if (Type == DatabaseProviderType.Sqlite)
                    return "Sqlite";
                return null;
            }
        }

        /// <summary>
        /// Объект соединения
        /// </summary>
        public DbConnection Connection()
        {
            if (Type == DatabaseProviderType.MsSql)
                return new SqlConnection(_connectionString);
            if (Type == DatabaseProviderType.PostgreSql)
                return new NpgsqlConnection(_connectionString);
            if (Type == DatabaseProviderType.Sqlite)
                return new SqliteConnection(_connectionString).AddUnicodeSupport();

            return null;
        }

        public AccountDbContext DbContext(Action<DbContextOptionsBuilder> optionsBuilderAction = null)
        {
            // TODO Turn on for unittests only
            //optionsBuilderAction = optionsBuilderAction ??
            //    (optionsBuilder =>
            //    {
            //        optionsBuilder.LogTo(message => Debug.WriteLine(message), new[] { DbLoggerCategory.Database.Command.Name });
            //        optionsBuilder.EnableSensitiveDataLogging();
            //    });
            if (Type == DatabaseProviderType.MsSql)
                return new MsSqlAccountDbContext(Connection(), optionsBuilderAction);
            if (Type == DatabaseProviderType.PostgreSql)
                return new PostgreSqlAccountDbContext(Connection(), optionsBuilderAction);
            if (Type == DatabaseProviderType.Sqlite)
                return new SqliteAccountDbContext(Connection(), optionsBuilderAction);
            return null;
        }

        public string FormatSchemaName(string schemaName)
        {
            if (Type == DatabaseProviderType.MsSql)
                return "[" + schemaName + "]";
            if (Type == DatabaseProviderType.PostgreSql)
                return "\"" + schemaName + "\"";
            if (Type == DatabaseProviderType.Sqlite)
                return null;
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

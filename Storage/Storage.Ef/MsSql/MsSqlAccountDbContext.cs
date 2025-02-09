using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Zidium.Storage.Ef
{
    internal class MsSqlAccountDbContext : AccountDbContext
    {
        public MsSqlAccountDbContext(
            DbConnection connection,
            Action<DbContextOptionsBuilder> optionsBuilderAction = null
            ) : base(connection, optionsBuilderAction)
        {
        }

        // For migrations
        public MsSqlAccountDbContext(DbContextOptions<MsSqlAccountDbContext> dbContextOptions) : base(dbContextOptions) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (Connection != null)
                optionsBuilder.UseSqlServer(Connection, t => t.CommandTimeout(60));
            else if (ConnectionString != null)
                optionsBuilder.UseSqlServer(ConnectionString, t => t.CommandTimeout(60));
        }

        public override DbConnection CreateConnection()
        {
            return base.CreateConnection() ?? new SqlConnection(ConnectionString);
        }

        public override long GetDatabaseSize()
        {
            DbConnection connection = null;
            try
            {
                connection = CreateConnection();

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = $"SELECT SUM(size) * 8 * 1024 as size FROM sys.master_files WHERE database_id = DB_ID();";
                    return (long)command.ExecuteScalar();
                }
            }
            finally
            {
                ReleaseConnection(connection);
            }
        }
    }
}

using System;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class PostgreSqlAccountDbContext : AccountDbContext
    {
        public PostgreSqlAccountDbContext(
            DbConnection connection,
            Action<DbContextOptionsBuilder> optionsBuilderAction = null
            ) : base(connection, optionsBuilderAction)
        {
        }

        // For migrations
        public PostgreSqlAccountDbContext(DbContextOptions<PostgreSqlAccountDbContext> dbContextOptions) : base(dbContextOptions) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (Connection != null)
                optionsBuilder.UseNpgsql(Connection, t => t.CommandTimeout(60));
            else if (ConnectionString != null)
                optionsBuilder.UseNpgsql(ConnectionString, t => t.CommandTimeout(60));
        }

        public override DbConnection CreateConnection()
        {
            return base.CreateConnection() ?? new NpgsqlConnection(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            base.OnModelCreating(modelBuilder);
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
                    command.CommandText = $"SELECT pg_database_size(current_database()) AS size;";
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

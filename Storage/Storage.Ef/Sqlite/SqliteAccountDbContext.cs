using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Zidium.Storage.Ef.Sqlite;

namespace Zidium.Storage.Ef
{
    internal class SqliteAccountDbContext : AccountDbContext
    {
        public SqliteAccountDbContext(
            DbConnection connection,
            Action<DbContextOptionsBuilder> optionsBuilderAction = null
            ) : base(connection, optionsBuilderAction)
        {
        }

        // For migrations
        public SqliteAccountDbContext(DbContextOptions<SqliteAccountDbContext> dbContextOptions) : base(dbContextOptions) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (Connection != null)
                optionsBuilder.UseSqlite(Connection, t => t.CommandTimeout(60));
            else if (ConnectionString != null)
                optionsBuilder.UseSqlite(ConnectionString, t => t.CommandTimeout(60));
        }

        public override DbConnection CreateConnection()
        {
            return base.CreateConnection() ?? new SqliteConnection(ConnectionString).AddUnicodeSupport();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            if (OnModelCreatingOverride != null)
                OnModelCreatingOverride.Invoke(modelBuilder);
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
                    command.CommandText = $"SELECT page_count * page_size AS size FROM pragma_page_count(), pragma_page_size();";
                    return (long)command.ExecuteScalar();
                }
            }
            finally
            {
                ReleaseConnection(connection);
            }
        }

        public static Action<ModelBuilder> OnModelCreatingOverride = null;
    }
}

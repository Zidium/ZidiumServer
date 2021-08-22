using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
    }
}

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Zidium.Storage.Ef
{
    internal class PostgreSqlAccountDbContext : AccountDbContext
    {
        public PostgreSqlAccountDbContext(DbConnection connection) : base(connection)
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

        public override AccountDbContext Clone()
        {
            var context = Provider.Current().DbContext(ConnectionString);
            return context;
        }

        public override DbConnection CreateConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            base.OnModelCreating(modelBuilder);
        }
    }
}

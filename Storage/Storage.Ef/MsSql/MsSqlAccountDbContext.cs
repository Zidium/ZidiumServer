using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Zidium.Storage.Ef
{
    internal class MsSqlAccountDbContext : AccountDbContext
    {
        public MsSqlAccountDbContext(DbConnection connection) : base(connection)
        {
        }

        // For migrations
        public MsSqlAccountDbContext(DbContextOptions<MsSqlAccountDbContext> dbContextOptions) : base(dbContextOptions) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Connection != null)
                optionsBuilder.UseSqlServer(Connection, t => t.CommandTimeout(60));
            else if (ConnectionString != null)
                optionsBuilder.UseSqlServer(ConnectionString, t => t.CommandTimeout(60));
        }

        public override AccountDbContext Clone()
        {
            var context = Provider.Current().DbContext(ConnectionString);
            return context;
        }

        public override DbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}

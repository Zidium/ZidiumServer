using System.Data.Common;
using Npgsql;

namespace Zidium.Core.AccountsDb
{
    public class PostgreSqlAccountDbContext : AccountDbContext
    {
        public PostgreSqlAccountDbContext() : base()
        {
        }

        public PostgreSqlAccountDbContext(DbConnection connection) : base(connection)
        {
        }

        public override AccountDbContext Clone()
        {
            var context = Provider.Current().DbContext(ConnectionString);
            context.DatabaseName = DatabaseName;
            return context;
        }

        public override DbConnection CreateConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }
    }
}

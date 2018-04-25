using System.Data.Common;
using System.Data.SqlClient;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class MsSqlAccountDbContext : AccountDbContext
    {
        public MsSqlAccountDbContext() : base()
        {
        }

        public MsSqlAccountDbContext(DbConnection connection) : base(connection)
        {
        }

        public override AccountDbContext Clone()
        {
            return Provider.Current().DbContext(ConnectionString);
        }

        public override DbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}

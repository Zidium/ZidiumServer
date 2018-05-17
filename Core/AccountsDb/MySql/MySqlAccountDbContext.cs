using System.Data.Common;
using System.Data.Entity;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;

namespace Zidium.Core.AccountsDb
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MySqlAccountDbContext : AccountDbContext
    {
        public MySqlAccountDbContext() : base()
        {
        }

        public MySqlAccountDbContext(DbConnection connection) : base(connection)
        {
        }

        public override AccountDbContext Clone()
        {
            return Provider.Current().DbContext(ConnectionString);
        }

        public override DbConnection CreateConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}

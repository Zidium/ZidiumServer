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
            var context = Provider.Current().DbContext(ConnectionString);
            context.DatabaseName = DatabaseName;
            return context;
        }

        public override DbConnection CreateConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}

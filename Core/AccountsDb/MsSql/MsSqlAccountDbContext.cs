﻿using System.Data.Common;
using System.Data.SqlClient;

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
            var context = Provider.Current().DbContext(ConnectionString);
            context.DatabaseName = DatabaseName;
            return context;
        }

        public override DbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}

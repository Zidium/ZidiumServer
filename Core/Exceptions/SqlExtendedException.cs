using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Zidium.Core
{
    public class SqlExtendedException : Exception
    {
        public SqlExtendedException(SqlException exception, DbCommand command)
            : base(null, exception)
        {
            Data["CommandText"] = command.CommandText;
            foreach (DbParameter parameter in command.Parameters)
            {
                Data["Parameter." + parameter.ParameterName] = parameter.Value;
            }

        }
    }
}

using System;
using System.Data.Common;

namespace Zidium.Common
{
    public class SqlExtendedException : Exception
    {
        public SqlExtendedException(DbException exception, DbCommand command)
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

using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Zidium.Core
{
    public static class SqlCommandHelper
    {
        public static int ExecuteNonQuery(DbCommand command)
        {
            var tryCount = 10;
            while (true)
            {
                try
                {
                    return command.ExecuteNonQuery();
                }
                catch (SqlException exception)
                {
                    tryCount--;
                    if (!CanRerun(exception.Message) || tryCount == 0)
                      throw new SqlExtendedException(exception, command);
                }
            }
        }

        public static bool CanRerun(string message)
        {
            return message.IndexOf("Rerun the transaction.", StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
    }
}

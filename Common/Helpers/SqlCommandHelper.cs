using System;
using System.Data.Common;

namespace Zidium.Common
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
                catch (DbException exception)
                {
                    tryCount--;
                    if (!CanRerun(exception.Message) || tryCount == 0)
                        throw new SqlExtendedException(exception, command);
                }
            }
        }

        public static bool CanRerun(string message)
        {
            return
                message.IndexOf("Rerun the transaction.", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                message.IndexOf("Deadlock", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                message.StartsWith("SQLite Error 5: 'database is locked'", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

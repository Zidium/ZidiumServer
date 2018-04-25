using System.Data.Common;
using System.Data.SqlClient;

namespace Zidium.Core
{
    public static class SqlCommandHelper
    {
        public static int ExecuteNonQuery(DbCommand command)
        {
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                throw new SqlExtendedException(exception, command);
            }
        }
    }
}

using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Zidium.Storage.Ef
{
    internal static class SqliteConnectionStringHelper
    {
        public static string SubstituteEnvVariables(string connectionString)
        {
            connectionString = connectionString.Replace("%localappdata%", Environment.GetEnvironmentVariable("localappdata"));

            var builder = new SqliteConnectionStringBuilder(connectionString);
            var path = Path.GetDirectoryName(builder.DataSource);
            Directory.CreateDirectory(path);

            return connectionString;
        }
    }
}

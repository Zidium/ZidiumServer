using System;
using Microsoft.Data.Sqlite;

namespace Zidium.Storage.Ef.Sqlite
{
    internal static class SqliteConnectionHelper
    {
        public static SqliteConnection AddUnicodeSupport(this SqliteConnection connection)
        {
            connection.CreateFunction("lower", (string x) => x?.ToLowerInvariant(), true);
            connection.CreateFunction("upper", (string x) => x?.ToUpperInvariant(), true);
            connection.CreateFunction("instr", (string x, string y) => x?.IndexOf(y, StringComparison.OrdinalIgnoreCase) + 1);
            connection.CreateCollation("UTF8CI", (x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));
            return connection;
        }
    }
}

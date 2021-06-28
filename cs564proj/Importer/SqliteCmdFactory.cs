using System;
using Microsoft.Data.Sqlite;

namespace Importer
{
    class SqliteCmdFactory : SqliteCommand
    {
        public static SqliteCommand CreateSqliteCmd(SqliteConnection conn, string cmdString)
        {
            SqliteCommand newCommand = conn.CreateCommand();
            newCommand.CommandText = cmdString;
            return newCommand;
        }

        public static SqliteParameter CreateSqliteParam(SqliteCommand cmd, string paramName)
        {
            SqliteParameter newParam = cmd.CreateParameter();
            newParam.ParameterName = paramName;
            return newParam;
        }
    }
}

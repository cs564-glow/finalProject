using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace LetterBoxDClone.Pages.Shared

{
    public class Connection
    {
        //TODO: replace with path to your database


        //private const string PATH_TO_DATABASE = "Data Source=/Users/Nugi/Downloads/hetrec2011-movielens-2k-v2/movie.db";

        private const string PATH_TO_DATABASE = "Data Source=C:\\ProgramData\\cs564proj\\movie.db";

        public static T GetSingleRow<T>(string query, Func<SqliteDataReader, T> function)
        {
            using var connection = new SqliteConnection(PATH_TO_DATABASE);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = query;

            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            T instance = function(reader);

            return instance;
        }

        public static List<T> GetMultipleRows<T>(string query, Func<SqliteDataReader, T> function)
        {
            using var connection = new SqliteConnection(PATH_TO_DATABASE);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = query;

            using SqliteDataReader reader = command.ExecuteReader();
            List<T> returnList = new List<T>();
            while (reader.Read())
            {
                T instance = function(reader);
                returnList.Add(instance);
            }

            return returnList;
        }
    }
}
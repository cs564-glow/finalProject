using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace LetterBoxDClone.Pages.Shared

{
    public class Connection
    {
        //TODO: replace with path to your database

        //private const string PATH_TO_DATABASE = "Data Source=/Users/Nugi/Downloads/hetrec2011-movielens-2k-v2/movie.db";

        private static string CONNECTION_STRING = "Data Source=" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cs564proj", "movie.db");

        /// <summary>
        /// Opens a connection to the database and returns a single 
        /// result from it that matches the passed querry. 
        /// </summary>
        /// <typeparam name="T">Type of the return object</typeparam>
        /// <param name="query">SQL query to execute</param>
        /// <param name="function">callback function reponsible for gathering
        /// data from SqliteDataReader object and returning it.
        /// Callback function should return an object of type T
        /// This function will be run on the first row returned by reader</param>
        /// <returns>Object returned by the callback function.
        /// Default of T if SQL query didn't return any results</returns>
        public static T GetSingleRow<T>(string query, Func<SqliteDataReader, T> function)
        {
            using var connection = new SqliteConnection(CONNECTION_STRING);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = query;

            using SqliteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                return function(reader);
            }

            return default;
        }

        public static int SetSingleRow(string query)
		{
            using var connection = new SqliteConnection(CONNECTION_STRING);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = query;

            return command.ExecuteNonQuery();
		}

        /// <summary>
        /// Opens a connection to the database and returns a list   
        /// containing results returned by the passed querry. 
        /// </summary>
        /// <typeparam name="T">Type of the return object</typeparam>
        /// <param name="query">SQL query to execute</param>
        /// <param name="function">callback function reponsible for gathering
        /// data from SqliteDataReader object and returning it.
        /// Callback function should return an object of type T.
        /// This function will be run on every row returned by reader</param>
        /// <returns>List containing objects returned by the callback function.
        /// An empty list if SQL query didn't return any results</returns>
        public static List<T> GetMultipleRows<T>(string query, Func<SqliteDataReader, T> function)
        {
            using var connection = new SqliteConnection(CONNECTION_STRING);
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
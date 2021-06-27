using System;
using System.IO;
using FileHelpers;
using Microsoft.Data.Sqlite;

namespace Importer
{
    class Program
    {
        // C:\ProgramData or /usr/share
        public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        public static string cs564proj = Path.Combine(appData, "cs564proj");
        static void Main(string[] args)
        {
            var engine = new FileHelperEngine<Movie>();
            var movies = engine.ReadFile("W:\\source\\repos\\finalProject\\testFiles\\ml-latest-small\\movies.csv");
            //foreach (var movie in movies)
            //{
            //    Console.Write(movie.Id + ", " + movie.Title + ", ");
            //    foreach (string genre in movie.GenreArray)
            //    {
            //        Console.Write(genre + ", ");
            //    }
            //    Console.WriteLine();
            //}

            Directory.CreateDirectory(cs564proj);
            using (var connection = new SqliteConnection("Data Source=" + cs564proj + "\\hello.db"))
            {
                connection.Open();

                var createMovieTableCmd = connection.CreateCommand();
                createMovieTableCmd.CommandText = @"CREATE TABLE movie (
movieId INTEGER PRIMARY KEY,
Title TEXT
);";
                createMovieTableCmd.ExecuteNonQuery();

                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"INSERT INTO movie(movieId, Title)
VALUES($movieId, $movieTitle);";

                    var movieIdParameter = command.CreateParameter();
                    movieIdParameter.ParameterName = "$movieId";
                    command.Parameters.Add(movieIdParameter);

                    var movieTitleParameter = command.CreateParameter();
                    movieTitleParameter.ParameterName = "$movieTitle";
                    command.Parameters.Add(movieTitleParameter);

                    foreach (var movie in movies)
                    {
                        //movieIdParameter.Value = movie.Id ?? DBNull.Value; // use nullable type?
                        //movieTitleParameter.Value = movie.Title ?? DBNull.Value;
                        movieIdParameter.Value = movie.Id;
                        movieTitleParameter.Value = movie.Title;
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}

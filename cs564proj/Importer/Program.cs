﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            FileHelperEngine<Movie> engine = new FileHelperEngine<Movie>();
            string path = "W:\\source\\repos\\finalProject\\testFiles\\ml-25m\\movies.csv";
            //string path = "W:\\source\\repos\\finalProject\\testFiles\\ml-latest-small\\movies.csv";
            Movie[] movies = engine.ReadFile(path);
            //foreach (var movie in movies)
            //{
            //    Console.Write(movie.Id + ", " + movie.Title + ", ");
            //    foreach (string genre in movie.GenreArray)
            //    {
            //        Console.Write(genre + ", ");
            //    }
            //    Console.WriteLine();
            //}

            // performance testing
            Stopwatch stopWatch = new Stopwatch();

            Directory.CreateDirectory(cs564proj);

            string connString = "Data Source=" + cs564proj + "\\hello-" + DateTime.Now.ToString("o").Replace(":",".") + ".db";
            using (SqliteConnection connection = new SqliteConnection(connString))
            {
                connection.Open();

                SqliteCommand createMovieTableCmd = connection.CreateCommand();
                createMovieTableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS movie (
movieId INTEGER PRIMARY KEY,
Title TEXT
);";
                createMovieTableCmd.ExecuteNonQuery();

                // method 1
//                using (SqliteTransaction transaction = connection.BeginTransaction())
//                {
//                    stopWatch.Start();
//                    // this works
//                    //                    SqliteCommand command = connection.CreateCommand();
//                    //                    command.CommandText = @"INSERT INTO movie(movieId, Title)
//                    //VALUES($movieId, $movieTitle);";

//                    //SqliteParameter movieIdParameter = command.CreateParameter();
//                    //movieIdParameter.ParameterName = "$movieId";
//                    //command.Parameters.Add(movieIdParameter);

//                    //SqliteParameter movieTitleParameter = command.CreateParameter();
//                    //movieTitleParameter.ParameterName = "$movieTitle";
//                    //command.Parameters.Add(movieTitleParameter);

//                    // this doesn't work because I can't access the members of Parameters
//                    //List <SqliteParameter> sqliteParams = new List<SqliteParameter>();
//                    //command.Parameters.AddRange(sqliteParams);

//                    // this works (trying to reduce duplicative code...)
//                    // I can't reduce any further because I can't access the members of Parameters
//                    // and so I need to create the SqliteParameter variables here.
//                    SqliteCommand insertCmd = SqliteCmdFactory.CreateSqliteCmd(connection, @"INSERT INTO movie(movieId, Title)
//VALUES($movieId, $movieTitle);");

//                    SqliteParameter movieIdParameter = SqliteCmdFactory.CreateSqliteParam(insertCmd, "$movieId");
//                    insertCmd.Parameters.Add(movieIdParameter);
//                    SqliteParameter movieTitleParameter = SqliteCmdFactory.CreateSqliteParam(insertCmd, "$movieTitle");
//                    insertCmd.Parameters.Add(movieTitleParameter);

//                    // didn't work
//                    //List<SqliteParameter> movieParams = new List<SqliteParameter>() { movieIdParameter, movieTitleParameter };
//                    //movieParams.AddRange(movieParams);

//                    foreach (Movie movie in movies)
//                    {
//                        // use nullable type?
//                        //movieIdParameter.Value = movie.Id ?? DBNull.Value; 
//                        //movieTitleParameter.Value = movie.Title ?? DBNull.Value;

//                        movieIdParameter.Value = movie.Id;
//                        movieTitleParameter.Value = movie.Title;

//                        //command.ExecuteNonQuery();
//                        insertCmd.ExecuteNonQuery();
//                    }

//                    transaction.Commit();
//                    stopWatch.Stop();
//                }

                // method 2
                using (SqliteTransaction transaction = connection.BeginTransaction())
                {
                    stopWatch.Start();

                    foreach (Movie movie in movies)
                    {
                        SqliteCommand insertCmd = SqliteCmdFactory.CreateSqliteCmd(connection, @"INSERT INTO movie(movieId, Title)
VALUES($movieId, $movieTitle);");
                        insertCmd.Parameters.AddWithValue("$movieId", movie.Id);
                        insertCmd.Parameters.AddWithValue("$movieTitle", movie.Title);

                        insertCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    stopWatch.Stop();
                }

            }
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }
    }
}

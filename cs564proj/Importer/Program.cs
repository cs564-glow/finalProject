﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Globalization;
using Microsoft.Data.Sqlite;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

namespace Importer
{
    class Program
    {
        // C:\ProgramData or /usr/share
        public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        public static string cs564proj = Path.Combine(appData, "cs564proj");
        static void Main(string[] args)
        {
            string path = "W:\\source\\repos\\finalProject\\testFiles\\hetrec2011-movielens-2k-v2\\movies.dat";
            List<Movie> movies;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t"
            };
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.TypeConverterCache.AddConverter<double?>(new NullableDoubleConverter());
                csv.Context.TypeConverterCache.AddConverter<int>(new NullableIntConverter());
                movies = csv.GetRecords<Movie>().ToList();
            }

            foreach (Movie movie in movies)
            {
                if (movie.RtAllCriticsRating is null)
                {
                    Console.Write(movie.MovieId + ", ");
                    Console.Write(movie.Title + ", ");
                    Console.Write(movie.Year + ", ");
                    Console.Write(movie.ImdbId + ", ");
                    Console.Write(movie.RtId + ", ");
                    Console.WriteLine(movie.RtAllCriticsRating);
                }
            }

            Directory.CreateDirectory(cs564proj);

            string connString = "Data Source=" + cs564proj + "\\hello-" + DateTime.Now.ToString("o").Replace(":", ".") + ".db";
            using (SqliteConnection connection = new SqliteConnection(connString))
            {
                connection.Open();

                SqliteCommand createMovieTableCmd = connection.CreateCommand();
                createMovieTableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS Movie (
MovieId INTEGER PRIMARY KEY,
Title TEXT,
ImdbId TEXT,
Year INTEGER,
RtId TEXT,
RtAllCriticsRating REAL,
RtAllCriticsNumReviews INTEGER
);";
                createMovieTableCmd.ExecuteNonQuery();
            }

            var contextOptions = new DbContextOptionsBuilder<MovieContext>()
                .UseSqlite(connString)
                .Options;

            using (var context = new MovieContext(contextOptions))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    context.BulkInsertOrUpdate(movies);
                    transaction.Commit();
                }
            }
        }

        /*
        static void MovieLensImport()
        {
            FileHelperEngine<MovieFileHelpers> engine = new FileHelperEngine<MovieFileHelpers>();
            string path = "W:\\source\\repos\\finalProject\\testFiles\\ml-25m\\movies.csv";
            //string path = "W:\\source\\repos\\finalProject\\testFiles\\ml-latest-small\\movies.csv";
            MovieFileHelpers[] movies = engine.ReadFile(path);

            // performance testing
            Stopwatch stopWatch = new Stopwatch();

            Directory.CreateDirectory(cs564proj);

            string connString = "Data Source=" + cs564proj + "\\hello-" + DateTime.Now.ToString("o").Replace(":", ".") + ".db";
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
                using (SqliteTransaction transaction = connection.BeginTransaction())
                {
                    stopWatch.Start();
                    SqliteCommand insertCmd = SqliteCmdFactory.CreateSqliteCmd(connection, @"INSERT INTO movie(movieId, Title)
VALUES($movieId, $movieTitle);");

                    // This was the best I could do to reduce duplicative code. AddRange didn't work.
                    // Creating the command in the foreach loop is extra work.
                    SqliteParameter movieIdParameter = SqliteCmdFactory.CreateSqliteParam(insertCmd, "$movieId");
                    SqliteParameter movieTitleParameter = SqliteCmdFactory.CreateSqliteParam(insertCmd, "$movieTitle");
                    insertCmd.Parameters.Add(movieIdParameter);
                    insertCmd.Parameters.Add(movieTitleParameter);

                    foreach (MovieFileHelpers movie in movies)
                    {
                        movieIdParameter.Value = movie.Id;
                        movieTitleParameter.Value = movie.Title;

                        insertCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    stopWatch.Stop();
                }

                // method 2 - slower
                //                using (SqliteTransaction transaction = connection.BeginTransaction())
                //                {
                //                    stopWatch.Start();

                //                    foreach (Movie movie in movies)
                //                    {
                //                        SqliteCommand insertCmd = SqliteCmdFactory.CreateSqliteCmd(connection, @"INSERT INTO movie(movieId, Title)
                //VALUES($movieId, $movieTitle);");
                //                        insertCmd.Parameters.AddWithValue("$movieId", movie.Id);
                //                        insertCmd.Parameters.AddWithValue("$movieTitle", movie.Title);

                //                        insertCmd.ExecuteNonQuery();
                //                    }

                //                    transaction.Commit();
                //                    stopWatch.Stop();
                //                }

            }
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        } */
    }

}

using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Usage: Importer -i <path to dataset> [-x]
    /// Pass -x to overwrite existing db
    /// </summary>
    class Program
    {
        // C:\ProgramData or /usr/share
        public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        public static string cs564proj = Path.Combine(appData, "cs564proj");
        static void Main(string[] args)
        {
            string movieDbFilepath = Path.Combine(cs564proj, "movie.db");
            string connString = "Data Source=" + movieDbFilepath;
            // "\\hello-" + DateTime.Now.ToString("o").Replace(":", ".") + ".db";
            bool shouldDeleteExisting = false;
            string datasetFolder = null;

            // Command-line parsing
            if (args.Length != 2 || args.Length != 3)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string arg = args[i];
                    if (arg.Equals("-i"))
                    { 
                        datasetFolder = args[++i];
                    }
                    else if (arg.Equals("-x"))
                    {
                        shouldDeleteExisting = true;
                    }
                }
            }
            else
            {
                Console.WriteLine("Usage: Importer -i <path to dataset> [-x] (-x to overwrite existing db)");
            }

            // bad arguments
            if (datasetFolder.Equals("") || datasetFolder is null)
            {
                // https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception?view=net-5.0#examples
                throw new ArgumentNullException("Error: Enter an input folder containing dataset after -i");
            }
            if (!Directory.Exists(datasetFolder))
            {
                throw new ArgumentException("Error: user-specified dataset folder does not exist");
            }

            Directory.CreateDirectory(cs564proj);

            // delete existing db
            if (File.Exists(movieDbFilepath) && shouldDeleteExisting)
            {
                File.Delete(movieDbFilepath);
            }

            // https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/#simple-dbcontext-initialization-with-new
            var contextOptions = new DbContextOptionsBuilder<MovieContext>()
                .UseSqlite(connString)
                .Options;

            using (var context = new MovieContext(contextOptions))
            {
                context.Database.EnsureCreated();
            }

            // CSV parsing
            string userTagsDat = Path.Combine(datasetFolder, "user_taggedmovies-timestamps.dat");
            string tagsDat = Path.Combine(datasetFolder, "tags.dat");
            string userRatingsDat = Path.Combine(datasetFolder, "user_ratedmovies-timestamps.dat");
            string movieDat = Path.Combine(datasetFolder, "movies.dat");
            string genreDat = Path.Combine(datasetFolder, "movie_genres.dat");

            importMovieDat(movieDat, connString, contextOptions);
            importGenreDat(genreDat, connString, contextOptions);
        }

        private static void importGenreDat(string genreDat, string connString, DbContextOptions<MovieContext> contextOptions)
        {
            List<Genre> genre;


        }

        /// <summary>
        /// Reads entire CSV file into memory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvFilepath"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        private static List<T> csvParsingAll<T>(string csvFilepath, string delimiter)
        {
            List<T> newList;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter
            };
            using (var reader = new StreamReader(csvFilepath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.TypeConverterCache.AddConverter<double?>(new NullableDoubleConverter());
                csv.Context.TypeConverterCache.AddConverter<int>(new NullableIntConverter());
                newList = csv.GetRecords<T>().ToList();
            }

            return newList;
        }

        private static void importMovieDat(string movieDat, string connString, DbContextOptions<MovieContext> contextOptions)
        {
            List<Movie> movies;

            // TODO: asynchronous CSV parsing
            movies = csvParsingAll<Movie>(movieDat, "\t");
            //var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            //{
            //    Delimiter = "\t"
            //};
            //using (var reader = new StreamReader(movieDat))
            //using (var csv = new CsvReader(reader, config))
            //{
            //    csv.Context.TypeConverterCache.AddConverter<double?>(new NullableDoubleConverter());
            //    csv.Context.TypeConverterCache.AddConverter<int>(new NullableIntConverter());
            //    movies = csv.GetRecords<Movie>().ToList();
            //}

            // SQLite Bulk Import
            // https://github.com/borisdj/EFCore.BulkExtensions
            using (var context = new MovieContext(contextOptions))
            {
                context.Database.EnsureCreated();

                using (var transaction = context.Database.BeginTransaction())
                {
                    context.BulkInsertOrUpdate(movies);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// DEPRECATED in favor of EFCore built-in functionality
        /// </summary>
        static void createMovieTableManual()
        {
            using (SqliteConnection connection = new SqliteConnection("Data Source=blahblah"))
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
        }
    }

}

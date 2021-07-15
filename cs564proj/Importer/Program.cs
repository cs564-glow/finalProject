using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
//using Microsoft.Data.Sqlite;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

namespace Importer
{
    /// <summary>
    /// Usage: Importer -i &lt;path to dataset&gt; [-x]
    /// Pass -x to overwrite existing db
    /// </summary>
    internal class Program
    {
        // C:\ProgramData or /usr/share
        private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        private static readonly string Cs564Proj = Path.Combine(AppData, "cs564proj");
        private static void Main(string[] args)
        {
            var movieDbFilepath = Path.Combine(Cs564Proj, "movie.db");
            var connString = "Data Source=" + movieDbFilepath;
            // "\\hello-" + DateTime.Now.ToString("o").Replace(":", ".") + ".db";
            var shouldDeleteExisting = false;
            string datasetFolder = null;

            // Command-line parsing
            if (args.Length != 2 || args.Length != 3)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
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
            if (datasetFolder is null || datasetFolder.Equals(""))
            {
                // https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception?view=net-5.0#examples
                throw new ArgumentNullException(nameof(datasetFolder));
            }
            if (!Directory.Exists(datasetFolder))
            {
                throw new ArgumentException("Error: user-specified dataset folder does not exist");
            }

            Directory.CreateDirectory(Cs564Proj);

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
            var userTagsDat = Path.Combine(datasetFolder, "user_taggedmovies-timestamps.dat");
            var tagsDat = Path.Combine(datasetFolder, "tags.dat");
            var userRatingsDat = Path.Combine(datasetFolder, "user_ratedmovies-timestamps.dat");
            var movieDat = Path.Combine(datasetFolder, "movies.dat");
            var genreDat = Path.Combine(datasetFolder, "movie_genres.dat");

            LoadMovieDat(movieDat, contextOptions);
            TransformLoadGenreDat(genreDat, contextOptions);
        }

        /// <summary>
        /// Transforms and loads movie_genres.dat into separate Movie and MovieGenre tables ENTIRELY IN MEMORY
        /// Could break up by transforming and loading in smaller batches.
        /// </summary>
        /// <param name="genreDat"></param>
        /// <param name="contextOptions"></param>
        private static void TransformLoadGenreDat(string genreDat, DbContextOptions<MovieContext> contextOptions)
        {
            var genreSet = new HashSet<Genre>();
            var genreDatList = new List<GenreDat>();

            // CSV Parsing
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t"
            };
            using (var reader = new StreamReader(genreDat))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    genreDatList.Add(csv.GetRecord<GenreDat>());

                    var newGenre = new Genre(csv.GetField("genre"));
                    genreSet.Add(newGenre);
                }
            }

            // Genre Category Import
            var genreList = genreSet.ToList();
            using (var context = new MovieContext(contextOptions))
            {
                context.Database.EnsureCreated();

                using (var transaction = context.Database.BeginTransaction())
                {
                    // https://github.com/borisdj/EFCore.BulkExtensions
                    // Quote - "SetOutputIdentity have purpose only when PK has Identity (usually int type with AutoIncrement),
                    // while if PK is Guid(sequential) created in Application there is no need for them."
                    context.BulkInsert(genreList, new BulkConfig { SetOutputIdentity = true });
                    transaction.Commit();
                }

                // Get back the list of auto-generated IDs and genre names
                // Yes, I could have just generated the IDs myself but...whatever I wanted to try autoincrement
                context.BulkRead(genreList);
            }

            // MovieGenre Transform
            // Use dictionary/hashtable for fast lookup
            var genreDict = new Dictionary<string, int>();
            foreach (var genre in genreList)
            {
                genreDict.Add(genre.GenreName, genre.GenreId);
            }
            var movieGenreList = new List<MovieGenre>();
            foreach (var movieGenreDat in genreDatList)
            {
                var movieGenre = new MovieGenre
                {
                    MovieId = movieGenreDat.MovieId, GenreId = genreDict[movieGenreDat.GenreName]
                };
                movieGenreList.Add(movieGenre);
            }

            // MovieGenre Import
            using (var context = new MovieContext(contextOptions))
            {
                context.Database.EnsureCreated();

                using (var transaction = context.Database.BeginTransaction())
                {
                    context.BulkInsert(movieGenreList);
                    transaction.Commit();
                }

            }
        }

        /// <summary>
        /// Reads entire CSV file into memory and returns a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvFilepath"></param>
        /// <param name="delimiter"></param>
        /// <returns>List of csv-parsed rows</returns>
        private static List<T> CsvParsingAll<T>(string csvFilepath, string delimiter)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter
            };
            using var reader = new StreamReader(csvFilepath);
            using var csv = new CsvReader(reader, config);
            csv.Context.TypeConverterCache.AddConverter<double?>(new NullableDoubleConverter());
            csv.Context.TypeConverterCache.AddConverter<int>(new NullableIntConverter());
            var newList = csv.GetRecords<T>().ToList();

            return newList;
        }

        private static void LoadMovieDat(string movieDat, DbContextOptions<MovieContext> contextOptions)
        {
            var movies = CsvParsingAll<Movie>(movieDat, "\t");

            // SQLite Bulk Import
            // https://github.com/borisdj/EFCore.BulkExtensions
            using var context = new MovieContext(contextOptions);
            context.Database.EnsureCreated();

            using var transaction = context.Database.BeginTransaction();
            context.BulkInsert(movies);
            transaction.Commit();
        }

/*
        /// <summary>
        /// DEPRECATED in favor of EFCore built-in functionality
        /// </summary>
        private static void CreateMovieTableManual()
        {
            using var connection = new SqliteConnection("Data Source=blahblah");
            connection.Open();

            var createMovieTableCmd = connection.CreateCommand();
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
*/
    }

}

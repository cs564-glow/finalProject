using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore; //using Microsoft.Data.Sqlite;

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
            if (datasetFolder is null or "")
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

            // dat files
            var userTagDat = Path.Combine(datasetFolder, "user_taggedmovies-timestamps.dat");
            var tagsDat = Path.Combine(datasetFolder, "tags.dat");
            var userRatingDat = Path.Combine(datasetFolder, "user_ratedmovies-timestamps.dat");
            var movieDat = Path.Combine(datasetFolder, "movies.dat");
            var genreDat = Path.Combine(datasetFolder, "movie_genres.dat");
            var actorDat = Path.Combine(datasetFolder, "movie_actors.dat");
            var directorDat = Path.Combine(datasetFolder, "movie_directors.dat");

            // load data
            LoadAllDatSimple<Movie>(movieDat, contextOptions);
            TransformLoadGenreDat(genreDat, contextOptions);
            LoadTagDat(tagsDat, contextOptions);

            var (userTagList, userSet) = ParseUserRatingOrTagDat<UserTag>(userTagDat, "\t");
            var (userRatingList, newUserSet) = ParseUserRatingOrTagDat<UserRating>(userRatingDat, "\t");
            userSet.UnionWith(newUserSet);
            LoadList(userTagList, contextOptions);
            LoadList(userRatingList, contextOptions);
            GenerateLoadUserData(userSet, contextOptions);

            var (actorList, castCrewSet) = ParseActorDirectorDat<ActsIn>(actorDat, "\t");
            LoadSet(castCrewSet, contextOptions);
            var (directorList, otherCastCrewSet) = ParseActorDirectorDat<Directs>(directorDat, "\t");
            castCrewSet.UnionWith(otherCastCrewSet);
            // Testing
            //foreach (var castCrew in castCrewSet)
            //{
            //    Console.WriteLine("id: " + castCrew.CastCrewId + ", name: " + castCrew.Name);
            //}

            //using (var writer = new StreamWriter("C:\\ProgramData\\cs564proj\\file.csv"))
            //using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            //{
            //    csv.WriteRecords(castCrewSet);
            //}

            var castCrewList = castCrewSet.ToList();
            LoadList(castCrewList, contextOptions);
            //LoadSet(castCrewSet, contextOptions);
            //LoadCastCrewSet(castCrewSet, contextOptions);
            LoadList(directorList, contextOptions);
            LoadList(actorList, contextOptions);
        }

        private static void LoadAllDatSimple<T>(string dat, DbContextOptions<MovieContext> contextOptions) where T : class
        {
            var parsedValsList = CsvParsingAll<T>(dat, "\t");
            using var context = new MovieContext(contextOptions);
            using var transaction = context.Database.BeginTransaction();
            context.BulkInsert(parsedValsList);
            transaction.Commit();
        }

        //private static void LoadActorDat(string actorDat, DbContextOptions<MovieContext> contextOptions)
        //{
        //    var directors = CsvParsingAll<Movie>(actorDat, "\t");

        //    // SQLite Bulk Import
        //    // https://github.com/borisdj/EFCore.BulkExtensions
        //    using var context = new MovieContext(contextOptions);
        //    using var transaction = context.Database.BeginTransaction();
        //    context.BulkInsert(directors);
        //    transaction.Commit();
        //}

        //private static void LoadDirectorDat(string directorDat, DbContextOptions<MovieContext> contextOptions)
        //{
        //    throw new NotImplementedException();
        //}

        private static void LoadSet<T>(HashSet<T> entitySet, DbContextOptions<MovieContext> contextOptions) where T : class
        {
            // TODO: reuse this for the other places where there is a generic set to add to list
            using var context = new MovieContext(contextOptions);
            using var transaction = context.Database.BeginTransaction();
            context.BulkInsert(entitySet.ToList());
            transaction.Commit();
        }
        //private static void LoadCastCrewSet(HashSet<CastCrew> entitySet, DbContextOptions<MovieContext> contextOptions)
        //{
        //    // TODO: reuse this for the other places where there is a generic set to add to list
        //    using var context = new MovieContext(contextOptions);
        //    using var transaction = context.Database.BeginTransaction();
        //    context.BulkInsert(entitySet.ToList());
        //    transaction.Commit();
        //}

        private static void GenerateLoadUserData(HashSet<User> userSet, DbContextOptions<MovieContext> contextOptions)
        {
            // TODO: generate random username and password data
            //foreach (var user in userSet)
            //{

            //}
            using var context = new MovieContext(contextOptions);
            using var transaction = context.Database.BeginTransaction();
            context.BulkInsert(userSet.ToList());
            transaction.Commit();
        }

        private static void LoadList<T>(IList<T> entityList, DbContextOptions<MovieContext> contextOptions) where T : class
        {
            //var (userTags, userSet) = ParseUserRatingOrTagDat<UserTag>(userTagDat, "\t");
            using var context = new MovieContext(contextOptions);
            using var transaction = context.Database.BeginTransaction();
            context.BulkInsert(entityList);
            transaction.Commit();
        }

        private static void LoadTagDat(string tagsDat, DbContextOptions<MovieContext> contextOptions)
        {
            if (contextOptions == null) throw new ArgumentNullException(nameof(contextOptions));
            var tags = CsvParsingAll<Tag>(tagsDat, "\t");
            using var context = new MovieContext(contextOptions);
            using var transaction = context.Database.BeginTransaction();
            context.BulkInsert(tags);
            transaction.Commit();
        }

        // TODO: Use reflection to make generic with ParseUserRatingOrTagDat
        private static (List<T> actorDirectorList, HashSet<CastCrew> castCrewSet) ParseActorDirectorDat<T>(string dat, string delimiter)
        {
            var actorDirectorList = new List<T>();
            //List<T> actorDirectorList;
            //if (typeof(T) == typeof(ActsIn))
            //{
            //}
            //else
            //{
            //    actorDirectorList = new List<Directs>() as List<T>;
            //}
            //var actorList = new List<ActsIn>();
            //var directorList = new List<Directs>();

            var castCrewSet = new HashSet<CastCrew>();

            // CSV Parsing
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
                Mode = CsvMode.NoEscape
            };
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = new StreamReader(dat, CodePagesEncodingProvider.Instance.GetEncoding(1252));
            using var csv = new CsvReader(reader, config);
            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                // this seems messy
                string castCrewId;
                string name;
                var movieId = csv.GetField<int>("movieID");
                var newItem = default(T);
                CastCrew newCastCrew = null;

                if (typeof(T) == typeof(ActsIn))
                {
                    castCrewId = csv.GetField("actorID").Trim();
                    name = csv.GetField("actorName");
                    var billing = csv.GetField<int>("ranking");
                    var newActsIn = new ActsIn(movieId, castCrewId, billing);
                    newItem = (T)(object)newActsIn;
                    //actorList.Add((new ActsIn(movieId, castCrewId, billing)));
                    newCastCrew = new CastCrew(castCrewId, name);
                }

                if (typeof(T) == typeof(Directs))
                {
                    castCrewId = csv.GetField<string>("directorID").Trim();
                    name = csv.GetField("directorName");
                    var newDirects = new Directs(movieId, castCrewId);
                    newItem = (T)(object)newDirects;
                    newCastCrew = new CastCrew(castCrewId, name);
                }

                actorDirectorList.Add(newItem);
                castCrewSet.Add(newCastCrew);

            }

            return (actorDirectorList, castCrewSet);
        }

        // TODO: Use reflection to make the userSet generic. This can be reused for loading genres, directors, and actors as well.
        private static (List<T> userTagOrRatingList, HashSet<User> userSet) ParseUserRatingOrTagDat<T>(string dat, string delimiter)
        {
            var userTagOrRatingList = new List<T>();
            var userSet = new HashSet<User>();

            // CSV Parsing
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter
            };
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = new StreamReader(dat, CodePagesEncodingProvider.Instance.GetEncoding(1252));
            using var csv = new CsvReader(reader, config);
            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                userTagOrRatingList.Add(csv.GetRecord<T>());
                userSet.Add(new User(csv.GetField<long>("userID")));
            }

            return (userTagOrRatingList, userSet);
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
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = new StreamReader(genreDat, CodePagesEncodingProvider.Instance.GetEncoding(1252));
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
                using var transaction = context.Database.BeginTransaction();
                // https://github.com/borisdj/EFCore.BulkExtensions
                // Quote - "SetOutputIdentity have purpose only when PK has Identity (usually int type with AutoIncrement),
                // while if PK is Guid(sequential) created in Application there is no need for them."
                context.BulkInsert(genreList, new BulkConfig { SetOutputIdentity = true });
                transaction.Commit();

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
                    MovieId = movieGenreDat.MovieId,
                    GenreId = genreDict[movieGenreDat.GenreName]
                };
                movieGenreList.Add(movieGenre);
            }

            // MovieGenre Import
            using (var context = new MovieContext(contextOptions))
            {
                using var transaction = context.Database.BeginTransaction();
                context.BulkInsert(movieGenreList);
                transaction.Commit();
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
                Delimiter = delimiter,
                Mode = CsvMode.NoEscape
            };
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = new StreamReader(csvFilepath, CodePagesEncodingProvider.Instance.GetEncoding(1252));
            using var csv = new CsvReader(reader, config);
            csv.Context.TypeConverterCache.AddConverter<double?>(new NullableDoubleConverter());
            csv.Context.TypeConverterCache.AddConverter<int>(new NullableIntConverter());
            var newList = csv.GetRecords<T>().ToList();

            return newList;
        }

/*
        private static void LoadMovieDat(string movieDat, DbContextOptions<MovieContext> contextOptions)
        {
            var movies = CsvParsingAll<Movie>(movieDat, "\t");

            // SQLite Bulk Import
            // https://github.com/borisdj/EFCore.BulkExtensions
            using var context = new MovieContext(contextOptions);
            using var transaction = context.Database.BeginTransaction();
            context.BulkInsert(movies);
            transaction.Commit();
        }
*/

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

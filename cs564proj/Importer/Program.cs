using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using EFCore.BulkExtensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using DataLibrary;

namespace Importer
{
    /// <summary>
    /// Usage: Importer -i &lt;path to dataset&gt; [-x] [-d] &lt;path to db&gt;
    /// Default movie.db location is on the desktop in cs564proj
    /// Pass -x to overwrite existing db
    /// Pass -d and a folder to change the default movie.db location
    /// </summary>
    internal class Program
    {
        // Environment.SpecialFolder.CommonApplicationData = C:\ProgramData or /usr/share
        // Environment.SpecialFolder.ApplicationData = C:\Users\john\AppData\Roaming or C:\Users\john
        // Desktop = C:\Users\john\Desktop or /Users/john/Desktop
        private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string Cs564Proj = Path.Combine(AppData, "cs564proj");
        private static void Main(string[] args)
        {
            var movieDbFilepath = Path.Combine(Cs564Proj, "movie.db");
            var connString = "Data Source=" + movieDbFilepath;
            // "\\hello-" + DateTime.Now.ToString("o").Replace(":", ".") + ".db";
            var shouldDeleteExisting = false;
            string datasetFolder = null;

            // Command-line parsing
            if (args.Length >= 2 && args.Length <= 5)
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
                    else if (arg.Equals("-d"))
                    {
                        movieDbFilepath = Path.Combine(args[++i], "movie.db");
                        connString = "Data Source=" + movieDbFilepath;
                    }
                }
            }
            else
            {
                Console.WriteLine("Usage: Importer -i <path to dataset> [-x] (-x to overwrite existing db) [-d] <change default movie.db location>");
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

            // Dat files
            var userTagDat = Path.Combine(datasetFolder, "user_taggedmovies-timestamps.dat");
            var tagsDat = Path.Combine(datasetFolder, "tags.dat");
            var userRatingDat = Path.Combine(datasetFolder, "user_ratedmovies-timestamps.dat");
            var movieDat = Path.Combine(datasetFolder, "movies.dat");
            var genreDat = Path.Combine(datasetFolder, "movie_genres.dat");
            var actorDat = Path.Combine(datasetFolder, "movie_actors.dat");
            var directorDat = Path.Combine(datasetFolder, "movie_directors.dat");
            var filmLocationDat = Path.Combine(datasetFolder, "movie_locations.dat");
            var countryProducedDat = Path.Combine(datasetFolder, "movie_countries.dat");

            //// Transform and Load
            var movieDatList = LoadAllDatSimple<Movie>(movieDat, contextOptions);
            ParseGenreDat(genreDat, contextOptions);
            LoadAllDatSimple<Tag>(tagsDat, contextOptions);

            // Transform
            var (userTagList, userSet) = ParseUserRatingOrTagDat<UserTag>(userTagDat, "\t");
            var (userRatingList, newUserSet) = ParseUserRatingOrTagDat<UserRating>(userRatingDat, "\t");
            userSet.UnionWith(newUserSet);
            // Load
            var generatedUserSet = GenerateUserData(userSet, userRatingList, movieDatList);
            BulkInsertSet(generatedUserSet, contextOptions);
            BulkInsertList(userTagList, contextOptions);
            BulkInsertList(userRatingList, contextOptions);

            // Transform
            var (actorList, castCrewSet) = ParseActorDirectorDat<ActsIn>(actorDat, "\t");
            var (directorList, otherCastCrewSet) = ParseActorDirectorDat<Directs>(directorDat, "\t");
            castCrewSet.UnionWith(otherCastCrewSet);
            // Load
            BulkInsertSet(castCrewSet, contextOptions);
            BulkInsertList(directorList, contextOptions);
            BulkInsertList(actorList, contextOptions);

            // Extract
            var (filmLocationDatList, countrySet) = ParseCountryAndLocation<FilmLocationDat>(filmLocationDat, "\t");
            var (countryProducedDatList, otherCountrySet) = ParseCountryAndLocation<CountryProducedDat>(countryProducedDat, "\t");
            countrySet.UnionWith(otherCountrySet);
            // Transform
            BulkInsertSet(countrySet, contextOptions);
            Dictionary<string, int> countryDict;
            using (var context = new MovieContext(contextOptions))
            {
                countryDict = context.Country.ToDictionary(c => c.Name, c => c.CountryId);
            }
            var filmLocationList = TransformFilmLocation(filmLocationDatList, countryDict);
            var countryProducedList = TransformCountryProduced(countryProducedDatList, countryDict);
            // Load
            BulkInsertList(filmLocationList, contextOptions);
            BulkUpdateCountryProducedManual(countryProducedList, connString);

            //
        }

        private static List<FilmLocation> TransformFilmLocation(List<FilmLocationDat> filmLocationDatList, Dictionary<string, int> countryDict)
        {
            var returnList = filmLocationDatList.Select(filmLocationDat => new FilmLocation
            {
                MovieId = filmLocationDat.MovieId,
                CountryId = filmLocationDat.Country != null && !filmLocationDat.Country.Equals("") ? countryDict[filmLocationDat.Country] : null,
                State = filmLocationDat.State,
                City = filmLocationDat.City,
                StreetAddress = filmLocationDat.StreetAddress
            })
                .ToList();
            return returnList;
        }
        private static List<CountryProduced> TransformCountryProduced(List<CountryProducedDat> countryProducedDatList, Dictionary<string, int> countryDict)
        {
            var returnList = countryProducedDatList.Select(countryProducedDat => new CountryProduced
            {
                MovieId = countryProducedDat.MovieId,
                CountryId = countryProducedDat.Country != null && !countryProducedDat.Country.Equals("") ? countryDict[countryProducedDat.Country] : null
            })
                .ToList();
            return returnList;
        }

        private static void BulkUpdateCountryProducedManual(IEnumerable<CountryProduced> countryProducedList, string connString)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/bulk-insert
            using var connection = new SqliteConnection(connString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
            UPDATE Movie
            SET CountryId = $CountryId
            WHERE MovieId = $MovieId
        ";

            var movieIdParam = command.CreateParameter();
            movieIdParam.ParameterName = "$MovieId";
            command.Parameters.Add(movieIdParam);

            var countryIdParam = command.CreateParameter();
            countryIdParam.ParameterName = "$CountryId";
            command.Parameters.Add(countryIdParam);

            // Insert a lot of data
            foreach (var countryProduced in countryProducedList)
            {
                if (countryProduced.CountryId is null)
                {
                    continue;
                }
                movieIdParam.Value = countryProduced.MovieId;
                countryIdParam.Value = countryProduced.CountryId;
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        private static List<T> LoadAllDatSimple<T>(string dat, DbContextOptions<MovieContext> contextOptions) where T : class
        {
            var parsedValsList = CsvParsingAll<T>(dat, "\t");
            BulkInsertList(parsedValsList, contextOptions);

            return parsedValsList;
        }

        private static void BulkInsertSet<T>(IEnumerable<T> entitySet, DbContextOptions<MovieContext> contextOptions) where T : class
        {
            var entityList = entitySet.ToList();
            BulkInsertList(entityList, contextOptions);
        }

        private static HashSet<User> GenerateUserData(HashSet<User> userSet, List<UserRating> userRatingList, List<Movie> movieDatList)
        {
            //using var context = new MovieContext(contextOptions);
            // Generate username and password data by reading from database - SLOW!!!
            //foreach (var user in userSet)
            //{
            //    var firstMovieId = context.UserRating.First(rating => rating.UserId == user.UserId).MovieId;
            //    var firstMovieTitle = TrimAllWithInplaceCharArray(context.Movie.Find(firstMovieId).Title);
            //    user.Username = firstMovieTitle + "Lover" + user.UserId;
            //    user.Password = firstMovieTitle + "123";
            //}
            // Generate username and password data from memory - FAST!!! as long as we have enough memory
            var movieDict = movieDatList.ToDictionary(m => m.MovieId, m => m.Title);
            var rand = new Random(123);

            foreach (var user in userSet)
            {
                //var firstMovieId = userRatingList.Select(rating => rating.UserId == user.UserId).ElementAt(rand.Next());
                var movieList = (from rating in userRatingList
                                 where rating.UserId == user.UserId
                                 select rating.MovieId).ToArray();
                var randMovieId = movieList[rand.Next(movieList.Count())];
                var randMovieTitle = TrimAllWithInplaceCharArray(movieDict[randMovieId]);
                user.Username = randMovieTitle + "Lover" + user.UserId;
                user.Password = randMovieTitle + "123";
            }

            return userSet;
        }

        private static void BulkInsertList<T>(IList<T> entityList, DbContextOptions<MovieContext> contextOptions) where T : class
        {
            using var context = new MovieContext(contextOptions);
            using var transaction = context.Database.BeginTransaction();
            context.BulkInsert(entityList);
            transaction.Commit();
        }

        // TODO: duplicative with User parsing and actor/director parsing
        private static (List<T> locationList, HashSet<Country> countrySet) ParseCountryAndLocation<T>(string dat, string delimiter)
        {
            var countrySet = new HashSet<Country>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
                Mode = CsvMode.NoEscape
            };
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = new StreamReader(dat, Encoding.GetEncoding(1252));
            using var csv = new CsvReader(reader, config);
            csv.Read();
            csv.ReadHeader();

            var returnList = new List<T>();

            while (csv.Read())
            {
                string countryName;

                if (typeof(T) == typeof(CountryProducedDat))
                {
                    countryName = csv.GetField("country");
                    returnList.Add((T)(object)new CountryProducedDat(csv.GetField<int>("movieID"), countryName));
                }
                else // Film Location
                {
                    countryName = csv.GetField("location1");
                    if (countryName is not null && !(countryName.Equals("")))
                    {
                        returnList.Add((T)(object)csv.GetRecord<FilmLocationDat>());
                    }
                }

                if (countryName is not null && !(countryName.Equals("")))
                {
                    countrySet.Add(new Country(countryName));
                }
            }

            return (returnList, countrySet);
        }

        // TODO: Use reflection to make generic with ParseUserRatingOrTagDat
        private static (List<T> actorDirectorList, HashSet<CastCrew> castCrewSet) ParseActorDirectorDat<T>(string dat, string delimiter)
        {
            var actorDirectorList = new List<T>();
            var castCrewSet = new HashSet<CastCrew>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
                Mode = CsvMode.NoEscape
            };
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = new StreamReader(dat, Encoding.GetEncoding(1252));
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

                    newItem = (T)(object)new ActsIn(movieId, castCrewId, billing);
                    newCastCrew = new CastCrew(castCrewId, name);
                }

                if (typeof(T) == typeof(Directs))
                {
                    castCrewId = csv.GetField<string>("directorID").Trim();
                    name = csv.GetField("directorName");

                    newItem = (T)(object)new Directs(movieId, castCrewId);
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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = new StreamReader(dat, Encoding.GetEncoding(1252));
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
        private static void ParseGenreDat(string genreDat, DbContextOptions<MovieContext> contextOptions)
        {
            var genreSet = new HashSet<Genre>();
            var genreDatList = new List<GenreDat>();

            // CSV Parsing
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t"
            };
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = new StreamReader(genreDat, Encoding.GetEncoding(1252));
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
            //var genreDict = new Dictionary<string, int>();
            //foreach (var genre in genreList)
            //{
            //    genreDict.Add(genre.GenreName, genre.GenreId);
            //}

            //var movieGenreList = new List<MovieGenre>();
            //foreach (var movieGenreDat in genreDatList)
            //{
            //    var movieGenre = new MovieGenre
            //    {
            //        MovieId = movieGenreDat.MovieId,
            //        GenreId = genreDict[movieGenreDat.GenreName]
            //    };
            //    movieGenreList.Add(movieGenre);
            //}

            var genreDict = genreList.ToDictionary(genre => genre.GenreName, genre => genre.GenreId);
            var movieGenreList = genreDatList.Select(movieGenreDat => new MovieGenre
            {
                MovieId = movieGenreDat.MovieId,
                GenreId = genreDict[movieGenreDat.GenreName]
            }).ToList();

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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = new StreamReader(csvFilepath, Encoding.GetEncoding(1252));
            using var csv = new CsvReader(reader, config);
            csv.Context.TypeConverterCache.AddConverter<double?>(new NullableDoubleConverter());
            csv.Context.TypeConverterCache.AddConverter<int>(new NullableIntConverter());
            var newList = csv.GetRecords<T>().ToList();

            return newList;
        }

        private static string TrimAllWithInplaceCharArray(string str)
        {
            var len = str.Length;
            var src = str.ToCharArray();
            int dstIdx = 0;
            for (int i = 0; i < len; i++)
            {
                var ch = src[i];
                if (!IsWhiteSpaceOrPunctuation(ch))
                    src[dstIdx++] = ch;
            }
            return new string(src, 0, dstIdx);
        }

        // https://stackoverflow.com/a/37347881
        // https://www.codeproject.com/Articles/1014073/Fastest-method-to-remove-all-whitespace-from-Strin
        // whitespace detection method: very fast, a lot faster than Char.IsWhiteSpace
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // if it's not inlined then it will be slow!!!
        private static bool IsWhiteSpaceOrPunctuation(char ch)
        {
            // this is surprisingly faster than the equivalent if statement
            switch (ch)
            {
                case '\u0009':
                case '\u000A':
                case '\u000B':
                case '\u000C':
                case '\u000D':
                case '\u0020':
                case '\u0085':
                case '\u00A0':
                case '\u1680':
                case '\u2000':
                case '\u2001':
                case '\u2002':
                case '\u2003':
                case '\u2004':
                case '\u2005':
                case '\u2006':
                case '\u2007':
                case '\u2008':
                case '\u2009':
                case '\u200A':
                case '\u2028':
                case '\u2029':
                case '\u202F':
                case '\u205F':
                case '\u3000':
                // look up other punctuation and remove
                // https://www.fileformat.info/info/unicode/category/Po/list.htm
                case '\x3A': // colon
                case '\u0021': // exclamation
                case '\u0022': // quotation
                case '\u0023': // number sign
                case '\u0025': // percent
                case '\u0026': // ampersand
                case '\u0027': // apostrophe
                case '\u002A': // asterisk
                case '\u002C': // comma
                case '\u002E': // full stop
                case '\u002F': // solidus
                case '\u003B': // semicolon
                case '\u003F': // question mark
                case '\u0040': // @
                case '\u005C': // REVERSE SOLIDUS
                    return true;
                default:
                    return false;
            }
        }

        // CURRENTLY UNUSED
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

        // CURRENTLY UNUSED
        /*private static void LoadCastCrewManual(HashSet<CastCrew> castCrewSet, string connString)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/bulk-insert
            using var connection = new SqliteConnection(connString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
            INSERT INTO CastCrew(CastCrewId, Name)
            VALUES ($CastCrewId, $Name)
        ";

            var castCrewIdParam = command.CreateParameter();
            castCrewIdParam.ParameterName = "$CastCrewId";
            command.Parameters.Add(castCrewIdParam);

            var nameParam = command.CreateParameter();
            nameParam.ParameterName = "$Name";
            command.Parameters.Add(nameParam);

            // Insert a lot of data
            foreach (var castCrew in castCrewSet)
            {
                castCrewIdParam.Value = castCrew.CastCrewId;
                nameParam.Value = castCrew.Name;
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }*/
    }
}

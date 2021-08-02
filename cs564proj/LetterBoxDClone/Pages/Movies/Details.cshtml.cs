using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary;
using LetterBoxDClone.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using static LetterBoxDClone.Pages.Shared.Connection;

namespace LetterBoxDClone.Pages
{
    public class MoviesModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public string MovieId { get; set; }
        public Movie Movie { get; set; }
        public CastCrew Director { get; set; }
        public List<CastCrew> CastAndCrew { get; set; }
        public List<Tag> mostAppliedTags { get; set; }
        public List<Genre> genres { get; set; }
        public List<Movie> similarMovies { get; set; }
        public string countryProduced { get; set; }
        public List<string> FilmingLocations { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(MovieId))
            {
                return Redirect("~/Search/AdvancedSearch");
            }

            Movie = GetSingleMovieByKey(MovieId);

            if (Movie == default(Movie))
            {
                return Redirect("~/Search/AdvancedSearch");
            }

            Director = GetDirectorByMovieKey(MovieId);
            CastAndCrew = GeMovieCastAndCrewByKey(MovieId);
            mostAppliedTags = GetMostAppliedTagsByKey(MovieId);
            genres = GetGenresByKey(MovieId);
            similarMovies = GetSimilarMovies(MovieId);
            countryProduced = GetCountryProducedByMovieKey(MovieId);
            FilmingLocations = GetFilmLocationsByMovieKey(MovieId);

            return Page();
        }

        public static Movie GetSingleMovieByKey(string MovieId)
        {
            string query =
                $@"
                 SELECT *
                 FROM Movie
                 WHERE MovieId = {MovieId}
                 ";

            Movie Movie = Connection.GetSingleRow<Movie>(query, GetMovieDataFromReader);

            return Movie;
        }

        public static Movie GetMovieDataFromReader(SqliteDataReader reader)
        {
            Movie movie = new Movie
            {
                MovieId = reader.GetInt32(0),
                Title = reader.GetString(1),
                Year = reader.GetString(2),
                CountryId = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                ImdbId = reader.GetString(4),
                RtId = reader.GetString(5),
                RtAllCriticsRating = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                RtAllCriticsNumReviews = reader.IsDBNull(7) ? 0 : reader.GetInt32(7)
            };

            return movie;
        }

        public static CastCrew GetDirectorByMovieKey(string MovieId)
        {
            string query =
                $@"
                SELECT cc.CastCrewId, cc.Name
                FROM Directs AS d
                NATURAL JOIN CastCrew AS cc
                WHERE d.MovieId = {MovieId}
                ";

            CastCrew Director = Connection.GetSingleRow(query, GetCrewDataFromReader);

            return Director;
        }

        public static List<CastCrew> GeMovieCastAndCrewByKey(string MovieId)
        {
            string query =
                $@"
                SELECT cc.CastCrewId, cc.Name
                FROM ActsIn AS ai
                NATURAL JOIN CastCrew AS cc
                WHERE ai.MovieId = {MovieId}
                ORDER BY ai.Billing
                ";

            List<CastCrew> castAndCrew = Connection.GetMultipleRows<CastCrew>(query, GetCrewDataFromReader);

            return castAndCrew;
        }

        public static CastCrew GetCrewDataFromReader(SqliteDataReader reader)
        {
            return new CastCrew(reader.GetString(0), reader.GetString(1));
        }

        public static List<Tag> GetMostAppliedTagsByKey(string MovieId)
        {
            string query =
                $@"
                SELECT t.TagId, t.Name
                FROM UserTag AS ut
                NATURAL JOIN Tag AS t
                WHERE ut.MovieId = {MovieId}
                GROUP BY ut.TagId
                ORDER BY count(*) DESC
                LIMIT 5
                ";

            List<Tag> mostAppliedTags = Connection.GetMultipleRows<Tag>(query, GetMostAppliedTagsFromReader);

            return mostAppliedTags;
        }

        public static Tag GetMostAppliedTagsFromReader(SqliteDataReader reader)
        {
            Tag tag = new Tag
            {
                TagId = reader.GetInt32(0),
                Name = reader.GetString(1)
            };

            return tag;
        }

        public static List<Genre> GetGenresByKey(string MovieId)
        {
            string query =
                $@"
                SELECT g.GenreId, g.GenreName
                FROM MovieGenre AS mg
                NATURAL JOIN Genre AS g
                WHERE mg.MovieId = {MovieId}
                ";

            List<Genre> genres = Connection.GetMultipleRows<Genre>(query, GetGenresFromReader);

            return genres;
        }

        public static Genre GetGenresFromReader(SqliteDataReader reader)
        {
            Genre genre = new Genre(reader.GetString(1))
            {
                GenreId = reader.GetInt32(0)
            };

            return genre;
        }

        public List<Movie> GetSimilarMovies(string MovieId)
        {
            string query =
                $@"
                SELECT
                        m1.MovieId,
                        m1.Title,
                        m1.Year,
                        m1.CountryId,
                        m1.ImdbId,
                        m1.RtId,
                        m1.RtAllCriticsRating,
                        m1.RtAllCriticsNumReviews
                FROM Movie as m1
                NATURAL JOIN   (SELECT ut.MovieID
                                FROM UserTag AS ut
                                WHERE ut.TagId IN  (SELECT t1.TagId
                                                    FROM UserTag AS ut1
                                                    NATURAL JOIN Tag as t1
                                                    WHERE ut1.MovieId = {MovieId}
                                                    GROUP BY ut1.TagId
                                                    ORDER BY count(*) DESC
                                                    LIMIT 5)
                                GROUP BY ut.MovieId, ut.TagID
                                ORDER BY count(*) DESC
                                LIMIT 5) similarMovies
                WHERE m1.MovieId <> {MovieId}
                ";

            List<Movie> similarMovies = Connection.GetMultipleRows(query, GetMovieDataFromReader);

            return similarMovies;
        }

        public static string GetCountryProducedByMovieKey(string MovieId)
        {
            string query =
                $@"
                SELECT c.Name
                FROM Movie AS m
                NATURAL JOIN Country AS c
                WHERE m.MovieId = {MovieId}
                ";

            string CountryProduced = Connection.GetSingleRow(query, GetCountryFromReader);

            return CountryProduced;
        }

        public static string GetCountryFromReader(SqliteDataReader reader)
        {
            return reader.GetString(0);
        }

        public static List<string> GetFilmLocationsByMovieKey(string MovieId)
        {
            string query =
                $@"
                SELECT fl.StreetAddress, fl.City, fl.State, c.Name
                FROM FilmLocation AS fl
                NATURAL JOIN Country AS c
                WHERE fl.MovieId = {MovieId}";

            List<string> filmingLocations = Connection.GetMultipleRows(query, GetFilmLocationsFromReader);

            return filmingLocations;
        }

        public static string GetFilmLocationsFromReader(SqliteDataReader reader)
        {
            string address = reader.GetString(0);
            string city = reader.GetString(1);
            string state = reader.GetString(2);
            string country = reader.GetString(3);

            return String.Join(", ", new List<string> { address, city, state, country }.Where(s => !string.IsNullOrEmpty(s)));
        }
    
    }
}

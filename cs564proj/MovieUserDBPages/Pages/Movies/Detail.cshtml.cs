using System;
using System.Collections.Generic;
using System.Reflection;
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
        public List<CastCrew> castAndCrew { get; set; }
        public List<Tag> mostAppliedTags { get; set; }
        public List<Genre> genres { get; set; }
        public List<Movie> similarMovies { get; set; }
        public string countryProduced { get; set; }

        public void OnGet()
        {
            //TODO: handle nulls
            Movie = GetSingleMovieByKey(MovieId);
            Director = GetDirectorByMovieKey(MovieId);
            castAndCrew = GeMovieCastAndCrewByKey(MovieId);
            mostAppliedTags = GetMostAppliedTagsByKey(MovieId);
            genres = GetGenresByKey(MovieId);
            similarMovies = GetSimilarMovies(MovieId);
            countryProduced = "placeholder"; //GetCountryProducedByMovieKey(MovieId);
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
            Movie movie = new Movie();

            movie.MovieId = reader.GetInt32(0);
            movie.Title = reader.GetString(1);
            movie.Year = reader.GetString(2);
            movie.ImdbId = reader.GetString(4);
            movie.RtId = reader.GetString(5);

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
            Tag tag = new Tag();

            tag.TagId = reader.GetInt32(0);
            tag.Name = reader.GetString(1);

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
            Genre genre = new Genre(reader.GetString(1));

            genre.GenreId = reader.GetInt32(0);

            return genre;
        }

        public List<Movie> GetSimilarMovies(string MovieId)
        {
            string query =
                $@"
                SELECT
                        m1.MovieId as MovieId,
                        m1.Title as Title,
                        m1.Year as Year,
                        m1.Year as Year,
                        m1.ImdbId as ImdbId,
                        m1.RtId as RtId
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
                FROM CountryProduced AS cp
                NATURAL JOIN Country AS c
                WHERE cp.MovieId = {MovieId}
                ";

            string CountryProduced = GetSingleRow(query, GetCountryFromReader);

            return CountryProduced;
        }

        public static string GetCountryFromReader(SqliteDataReader reader)
        {
            return reader.GetString(0);
        }
    }
}

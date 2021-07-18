using System;
using System.Collections.Generic;
using System.Reflection;
using Importer;
using LetterBoxDClone.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using static LetterBoxDClone.Pages.Shared.Connection;

namespace LetterBoxDClone.Pages
{
    public class MoviesModel : PageModel
    {

        [BindProperty(SupportsGet =true)]
        public string MovieId { get; set; }
        public Movie Movie { get; set; }
        public List<string> castAndCrew { get; set; }
        public List<string> mostAppliedTags { get; set; }
        public List<string> genres { get; set; }
        public List<Movie> similarMovies { get; set; }

        public void OnGet()
        {
            //TODO: handle nulls
            Movie = GetSingleMovieByKey(MovieId);
            castAndCrew = GeMovieCastAndCrewByKey(MovieId);
            mostAppliedTags = GetMostAppliedTagsByKey(MovieId);
            genres = GetGenresByKey(MovieId);
            similarMovies = GetSimilarMovies(MovieId);
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
            movie.ImdbId = reader.GetString(2);
            movie.Year = reader.GetString(3);
            movie.RtId = reader.GetString(4);

            return movie;
        }

        public static List<string> GeMovieCastAndCrewByKey(string MovieId)
        {
            string query =
                $@"
                SELECT *
                FROM ActsIn
                WHERE MovieId = {MovieId}
                ORDER BY Ranking
                ";

            List<string> castAndCrew = Connection.GetMultipleRows<string>(query, GetCrewDataFromReader);

            return castAndCrew;
        }

        public static string GetCrewDataFromReader(SqliteDataReader reader)
        {
            return reader.GetString(2);
        }

        public static List<string> GetMostAppliedTagsByKey(string MovieId)
        {
            string query =
                $@"
                SELECT t.Name
                FROM UserTag AS ut
                NATURAL JOIN Tag AS t
                WHERE ut.MovieId = {MovieId}
                GROUP BY ut.TagId
                ORDER BY count(*) DESC
                LIMIT 5
                ";

            List<string> mostAppliedTags = Connection.GetMultipleRows<string>(query, GetMostAppliedTagsFromReader);

            return mostAppliedTags;
        }

        public static string GetMostAppliedTagsFromReader(SqliteDataReader reader)
        {
            return reader.GetString(0);
        }

        public static List<string> GetGenresByKey(string MovieId)
        {
            string query =
                $@"
                SELECT g.GenreName
                FROM MovieGenre AS mg
                NATURAL JOIN Genre AS g
                WHERE mg.MovieId = {MovieId}
                ";

            List<string> genres = Connection.GetMultipleRows<string>(query, GetGenresFromReader);

            return genres;
        }

        public static string GetGenresFromReader(SqliteDataReader reader)
        {
            return reader.GetString(0);
        }

        public List<Movie> GetSimilarMovies(string MovieId)
        {
            //TODO: check this query after movie key thing has been fixed. Right now it doesn't make any sense
            string query =
                $@"
                SELECT
                        m1.MovieId as MovieId,
                        m1.Title as Title,
                        m1.ImdbId as ImdbId,
                        m1.Year as Year,
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
                WHERE m1.MovieId <> {MovieId}"; 

            List<Movie> similarMovies = Connection.GetMultipleRows(query, GetMovieDataFromReader);

            foreach (Movie movie in similarMovies)
            {
                Console.WriteLine(movie.Title);
            }

            return similarMovies;
        }
    }
}

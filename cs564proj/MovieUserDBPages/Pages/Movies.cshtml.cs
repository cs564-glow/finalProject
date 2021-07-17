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

        public string MovieId { get; set; }
        public Movie Movie { get; set; }
        public List<string> castAndCrew { get; set; }

        public void OnGet()
        {
            //TODO: handle nulls
            Movie = GetSingleMovieByKey(MovieId);
            castAndCrew = GeMovieCastAndCrew(MovieId);
        }

        public static Movie GetSingleMovieByKey(string MovieId)
        {
            string query =
                $@"
                 SELECT *
                 FROM Movie
                 WHERE MovieId = {MovieId}
                 ";

            Movie Movie = Connection.GetSingleRow<Movie>(query, GetMovieData);

            return Movie;
        }

        public static Movie GetMovieData(SqliteDataReader reader)
        {
            Movie movie = new Movie();

            movie.MovieId = reader.GetInt32(0);
            movie.Title = reader.GetString(1);
            movie.ImdbId = reader.GetString(2);
            movie.Year = reader.GetString(3);
            movie.RtId = reader.GetString(4);

            return movie;
        }

        public static List<string> GeMovieCastAndCrew(string MovieId)
        {
            List<string> castAndCrew; // = new List<string>();
            //castAndCrew.Add("Michael Fassbender");
            //castAndCrew.Add("Carey Mulligan");

            string query =
                $@"
                SELECT *
                FROM ActsIn
                WHERE MovieId = {MovieId}
                ORDER BY Ranking
                ";

            castAndCrew = Connection.GetMultipleRows<string>(query, GetCrewData);

            return castAndCrew;
        }

        public static string GetCrewData(SqliteDataReader reader)
        {
            return reader.GetString(2);
        }

    }
}

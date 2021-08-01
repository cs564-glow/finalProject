using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary;
using LetterBoxDClone.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace LetterBoxDClone.Pages.Search
{
    public class AdvancedSearchModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string MovieTitle { get; set; }
        [BindProperty(SupportsGet = true)]
        public int MovieYear { get; set; }
        [BindProperty(SupportsGet = true)]
        public string CastCrew { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Director { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Genre { get; set; }
        [BindProperty(SupportsGet = true)]
        public string CountryProduced { get; set; }
        [BindProperty(SupportsGet = true)]
        public string CountryFilmed { get; set; }
        [BindProperty(SupportsGet = true)]
        public string StateFilmed { get; set; }
        [BindProperty(SupportsGet = true)]
        public string CityFilmed { get; set; }
        [BindProperty(SupportsGet = true)]
        public string AddressFilmed { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Tag { get; set; }

        public List<Movie> searchResults;

        public void OnGet()
        {
            //for every search criteria passed, this list will have an entry
            //we will later join all of these queries with NATURAL JOIN
            List<String> logicToUse = new List<string>();

            string query;

            if (!String.IsNullOrEmpty(MovieTitle))
            {
                query =
                 $@"(
                 SELECT *
                 FROM Movie
                 WHERE Title LIKE '%{MovieTitle}%'
                 )";

                logicToUse.Add(query);
            }

            if (MovieYear > 0)
            {
                query =
                $@"(
                 SELECT DISTINCT MovieId
                 FROM Movie
                 WHERE Year = '{MovieYear}'
                 )";

                logicToUse.Add(query);
            }

            if (!String.IsNullOrEmpty(CastCrew))
            {
                query =
                $@"(
                SELECT DISTINCT ai.MovieId
                FROM ActsIn AS ai
                NATURAL JOIN CastCrew AS cc
                WHERE cc.Name LIKE '%{CastCrew}%'
                )";

                logicToUse.Add(query);
            }

            if (!String.IsNullOrEmpty(Director))
            {
                query =
                $@"(
                SELECT DISTINCT d.MovieId
                FROM Directs AS d
                NATURAL JOIN CastCrew AS cc
                WHERE cc.Name LIKE '%{Director}%'
                )";

                logicToUse.Add(query);
            }

            if (!String.IsNullOrEmpty(Genre))
            {
                query =
                $@"(
                SELECT DISTINCT mg.MovieId
                FROM Genre AS g
                NATURAL JOIN MovieGenre AS mg
                WHERE g.GenreName LIKE '%{Genre}%'
                )";

                logicToUse.Add(query);
            }

            if (!String.IsNullOrEmpty(CountryProduced))
            {
                query =
                $@"(
                SELECT DISTINCT m.MovieId
                FROM Movie AS m
                NATURAL JOIN Country AS c
                WHERE c.Name LIKE '%{CountryProduced}%'
                )";

                logicToUse.Add(query);
            }

            if (!String.IsNullOrEmpty(Tag))
            {
                query =
                $@"(
                SELECT DISTINCT ut.MovieId
                FROM Tag AS t
                NATURAL JOIN UserTag AS ut
                WHERE t.Name LIKE '%{Tag}%'
                )";

                logicToUse.Add(query);
            }

            if (!String.IsNullOrEmpty(CountryFilmed))
            {
                query =
                $@"(
                SELECT DISTINCT fl.MovieId
                FROM FilmLocation AS fl
                NATURAL JOIN Country AS c
                WHERE c.Name LIKE '%{CountryFilmed}%'
                )";

                logicToUse.Add(query);
            }

            if (!String.IsNullOrEmpty(StateFilmed))
            {
                query =
                $@"(
                SELECT DISTINCT fl.MovieId
                FROM FilmLocation AS fl
                WHERE fl.State LIKE '%{StateFilmed}%'
                )";

                logicToUse.Add(query);
            }

            if (!String.IsNullOrEmpty(CityFilmed))
            {
                query =
                $@"(
                SELECT DISTINCT fl.MovieId
                FROM FilmLocation AS fl
                WHERE fl.City LIKE '%{CityFilmed}%'
                )";

                logicToUse.Add(query);
            }

            if (!String.IsNullOrEmpty(AddressFilmed))
            {
                query =
                $@"(
                SELECT DISTINCT fl.MovieId
                FROM FilmLocation AS fl
                WHERE fl.StreetAddress LIKE '%{AddressFilmed}%'
                )";

                logicToUse.Add(query);
            }

            //If we have any logic, join all of those using NATURAL JOIN, and then
            //NATURAL JOIN it on movies table to get final results and all columns
            if (logicToUse.Count > 0)
            {
                query =
                $@"
                SELECT *
                FROM Movie
                ";

                string logicQuery = string.Join(" NATURAL JOIN ", logicToUse);
                string fq = string.Join("NATURAL JOIN ", query, logicQuery);

                searchResults = Connection.GetMultipleRows(fq, MoviesModel.GetMovieDataFromReader);
            }

        }
    }
}

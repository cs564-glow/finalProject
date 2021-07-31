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
            string movieQuery =
                 $@"
                 SELECT *
                 FROM Movie
                 ";

            string movieTitleQuery =
                 $@"(
                 SELECT *
                 FROM Movie
                 WHERE Title LIKE '%{MovieTitle}%'
                 )";

            string movieYearQuery =
                 $@"(
                 SELECT DISTINCT MovieId
                 FROM Movie
                 WHERE Year = '{MovieYear}'
                 )";

            string castCrewQuery =
                $@"(
                SELECT DISTINCT ai.MovieId
                FROM ActsIn AS ai
                NATURAL JOIN CastCrew AS cc
                WHERE cc.Name LIKE '%{CastCrew}%'
                )";

            string directorQuery =
                $@"(
                SELECT DISTINCT d.MovieId
                FROM Directs AS d
                NATURAL JOIN CastCrew AS cc
                WHERE cc.Name LIKE '%{Director}%'
                )";

            string genreQuery =
                $@"(
                SELECT DISTINCT mg.MovieId
                FROM Genre AS g
                NATURAL JOIN MovieGenre AS mg
                WHERE g.GenreName LIKE '%{Genre}%'
                )";

            string countryProducedQuery =
                $@"(
                SELECT DISTINCT m.MovieId
                FROM Movie AS m
                NATURAL JOIN Country AS c
                WHERE c.Name LIKE '%{CountryProduced}%'
                )";

            string tagQuery =
                $@"(
                SELECT DISTINCT ut.MovieId
                FROM Tag AS t
                NATURAL JOIN UserTag AS ut
                WHERE t.Name LIKE '%{Tag}%'
                )";

            string countryFilmedQuery =
                $@"(
                SELECT DISTINCT fl.MovieId
                FROM FilmLocation AS fl
                WHERE c.Name LIKE '%{CountryFilmed}%'
                )";

            string stateFilmedQuery =
                $@"(
                SELECT DISTINCT fl.MovieId
                FROM FilmLocation AS fl
                WHERE fl.State LIKE '%{StateFilmed}%'
                )";

            string addressFilmedQuery =
                $@"(
                SELECT DISTINCT fl.MovieId
                FROM FilmLocation AS fl
                WHERE fl.StreetAddress LIKE '%{AddressFilmed}%'
                )";

            string cityFilmedQuery =
                $@"(
                SELECT DISTINCT fl.MovieId
                FROM FilmLocation AS fl
                WHERE fl.City LIKE '%{CityFilmed}%'
                )";

            List<String> list = new List<string>();

            if (!String.IsNullOrEmpty(MovieTitle))
            {
                list.Add(movieTitleQuery);
            }

            if (MovieYear > 0)
            {
                list.Add(movieYearQuery);
            }

            if (!String.IsNullOrEmpty(CastCrew))
            {
                list.Add(castCrewQuery);
            }

            if (!String.IsNullOrEmpty(Director))
            {
                list.Add(directorQuery);
            }

            if (!String.IsNullOrEmpty(Genre))
            {
                list.Add(genreQuery);
            }

            if (!String.IsNullOrEmpty(CountryProduced))
            {
                list.Add(countryProducedQuery);
            }

            if (!String.IsNullOrEmpty(Tag))
            {
                list.Add(tagQuery);
            }

            if (!String.IsNullOrEmpty(CountryFilmed))
            {
                list.Add(countryFilmedQuery);
            }

            if (!String.IsNullOrEmpty(StateFilmed))
            {
                list.Add(stateFilmedQuery);
            }

            if (!String.IsNullOrEmpty(CityFilmed))
            {
                list.Add(cityFilmedQuery);
            }

            if (!String.IsNullOrEmpty(AddressFilmed))
            {
                list.Add(addressFilmedQuery);
            }

            if (list.Count > 0)
            {
                string fq = string.Join("NATURAL JOIN ", movieQuery, string.Join(" NATURAL JOIN ", list));
                //Console.WriteLine(fq);
                searchResults = Connection.GetMultipleRows(fq, MoviesModel.GetMovieDataFromReader);

                //Console.WriteLine();
                //foreach (Movie movie in movies)
                //{
                //    Console.WriteLine(movie.MovieId + ": " + movie.Title);
                //}
            }
            else
            {
                return;
            }

        }

        public string QueryConstructor(Dictionary<string, string> searchParameters)
        {
            List<String> list = new List<string>();

            if (!String.IsNullOrEmpty(MovieTitle))
            {
                list.Add($"Title LIKE '%{MovieTitle}%");
            }

            if (MovieYear > 0)
            {
                list.Add($"WHERE Year = '{MovieYear}'");
            }

            //foreach (KeyValuePair<string, string> kvp in searchParameters)
            //{
            //    Director = "a";
            //}
                return null;
        }

        public static List<Movie> GetMovies(string query)
        {
            //TODO: Can probably just do away with this tag
            List<Movie> movies = Connection.GetMultipleRows(query, MoviesModel.GetMovieDataFromReader);

            foreach (Movie movie in movies)
            {
                Console.WriteLine(movie.Title);
            }

            return null;
        }
    }
}

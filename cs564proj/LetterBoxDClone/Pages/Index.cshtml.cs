using DataLibrary;
using LetterBoxDClone.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetterBoxDClone.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<Movie> MostSeen { get; set; }
        public List<Movie> HighestRated { get; set; }
        public List<CastCrew> ActorsWithHighestRatedMovies { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            GetMostSeenMovies();
            GetHighestRatedMovies();
            GetActorsWithHighestRatedMovies();
        }

        private void GetMostSeenMovies()
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
                FROM Movie AS m1
                NATURAL JOIN UserRating AS ur
                GROUP BY ur.MovieId
                HAVING count(*) > 10
                ORDER BY count(*) DESC
                LIMIT 10
                ";

            MostSeen = Connection.GetMultipleRows(query, MoviesModel.GetMovieDataFromReader);
        }

        private void GetHighestRatedMovies()
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
                FROM Movie AS m1
                NATURAL JOIN UserRating AS ur
                GROUP BY ur.MovieId
                HAVING count(*) > 10
                ORDER BY AVG(Rating) DESC
                LIMIT 10
                ";

            HighestRated = Connection.GetMultipleRows(query, MoviesModel.GetMovieDataFromReader);  
        }

        private void GetActorsWithHighestRatedMovies()
        {
            string query =
                $@"
                SELECT
                    cc.CastCrewId,
	                cc.Name,
	                AVG(avgRating)
                FROM ActsIn AS ai
                NATURAL JOIN 
	                (SELECT
		                ur.MovieId,
		                AVG(ur.Rating) avgRating
	                FROM UserRating AS ur
	                GROUP BY (ur.MovieId)
                    HAVING count(*) > 10) AS amr
                NATURAL JOIN CastCrew AS cc
                GROUP BY cc.CastCrewId
                HAVING count(*) > 3
                ORDER BY AVG(avgRating) DESC
                LIMIT 10
                ";

            ActorsWithHighestRatedMovies = Connection.GetMultipleRows(query, GetUsersWithMostRatingsFromReader);
        }

        private CastCrew GetUsersWithMostRatingsFromReader(SqliteDataReader reader)
        {
            return new CastCrew(reader.GetString(0), reader.GetString(1));
        }
    }
}

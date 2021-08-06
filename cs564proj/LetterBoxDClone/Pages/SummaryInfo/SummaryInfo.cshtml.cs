using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary;
using LetterBoxDClone.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LetterBoxDClone.Pages.SummaryInfo
{
    public class SummaryModel : PageModel
    {
        private readonly MovieContext _context;
        public SummaryModel(MovieContext context)
        {
            _context = context;
        }

        public int CountMovie { get; set; }
        //public int CountCastCrew { get; set; }
        //public int CountCountry { get; set; }
        //public int CountGenre { get; set; }
        //public int CountUser { get; set; }
        //public int CountUserTag { get; set; }
        //public int CountUserRating { get; set; }
        //public int CountFilmLocation { get; set; }
        //public int CountActsIn { get; set; }
        //public int CountDirects { get; set; }
        public Dictionary<string, int> summaryInfoDictionary { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            summaryInfoDictionary = new Dictionary<string, int>();
            CountMovie = await _context.Movie.Select(m => m.MovieId).CountAsync();
            summaryInfoDictionary.Add("Movie", CountMovie);

            summaryInfoDictionary.Add("Genre", await _context.Genre.Select(m => m.GenreId).CountAsync());
            summaryInfoDictionary.Add("MovieGenre", await _context.MovieGenre.Select(m => m.MovieId).CountAsync());
            summaryInfoDictionary.Add("CastCrew", await _context.CastCrew.Select(m => m.CastCrewId).CountAsync());
            summaryInfoDictionary.Add("ActsIn", await _context.ActsIn.Select(m => m.CastCrewId).CountAsync());
            summaryInfoDictionary.Add("Directs", await _context.Directs.Select(m => m.CastCrewId).CountAsync());
            summaryInfoDictionary.Add("User", await _context.User.Select(m => m.UserId).CountAsync());
            summaryInfoDictionary.Add("UserTag", await _context.UserTag.Select(m => m.UserId).CountAsync());
            summaryInfoDictionary.Add("Tag", await _context.Tag.CountAsync());
            summaryInfoDictionary.Add("UserRating", await _context.UserRating.CountAsync());
            summaryInfoDictionary.Add("Country", await _context.CastCrew.Select(m => m.CastCrewId).CountAsync());
            summaryInfoDictionary.Add("FilmLocation", await _context.FilmLocation.CountAsync());

            if (summaryInfoDictionary == null)
            {
                return NotFound();
            }



            return Page();
        }

        //public List<Movie> MostSeen { get; set; }
        //public List<Movie> HighestRated { get; set; }
        //public List<CastCrew> ActorsWithHighestRatedMovies { get; set; }

        //private void GetMostSeenMovies()
        //{
        //    string query =
        //        $@"
        //        SELECT
        //                m1.MovieId,
        //                m1.Title,
        //                m1.Year,
        //                m1.CountryId,
        //                m1.ImdbId,
        //                m1.RtId,
        //                m1.RtAllCriticsRating,
        //                m1.RtAllCriticsNumReviews
        //        FROM Movie AS m1
        //        NATURAL JOIN UserRating AS ur
        //        GROUP BY ur.MovieId
        //        HAVING count(*) > 10
        //        ORDER BY count(*) DESC
        //        LIMIT 10
        //        ";

        //    MostSeen = Connection.GetMultipleRows(query, MoviesModel.GetMovieDataFromReader);
        //}

        //private void GetHighestRatedMovies()
        //{
        //    string query =
        //        $@"
        //        SELECT
        //                m1.MovieId,
        //                m1.Title,
        //                m1.Year,
        //                m1.CountryId,
        //                m1.ImdbId,
        //                m1.RtId,
        //                m1.RtAllCriticsRating,
        //                m1.RtAllCriticsNumReviews
        //        FROM Movie AS m1
        //        NATURAL JOIN UserRating AS ur
        //        GROUP BY ur.MovieId
        //        HAVING count(*) > 10
        //        ORDER BY AVG(Rating) DESC
        //        LIMIT 10
        //        ";

        //    HighestRated = Connection.GetMultipleRows(query, MoviesModel.GetMovieDataFromReader);
        //}

        //private void GetActorsWithHighestRatedMovies()
        //{
        //    string query =
        //        $@"
        //        SELECT
        //            cc.CastCrewId,
	       //         cc.Name,
	       //         AVG(avgRating)
        //        FROM ActsIn AS ai
        //        NATURAL JOIN 
	       //         (SELECT
		      //          ur.MovieId,
		      //          AVG(ur.Rating) avgRating
	       //         FROM UserRating AS ur
	       //         GROUP BY (ur.MovieId)
        //            HAVING count(*) > 10) AS amr
        //        NATURAL JOIN CastCrew AS cc
        //        GROUP BY cc.CastCrewId
        //        HAVING count(*) > 3
        //        ORDER BY AVG(avgRating) DESC
        //        LIMIT 10
        //        ";

        //    ActorsWithHighestRatedMovies = Connection.GetMultipleRows(query, GetUsersWithMostRatingsFromReader);
        //}

        //private CastCrew GetUsersWithMostRatingsFromReader(SqliteDataReader reader)
        //{
        //    return new CastCrew(reader.GetString(0), reader.GetString(1));
        //}
    }
}

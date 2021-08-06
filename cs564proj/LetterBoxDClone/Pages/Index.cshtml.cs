using DataLibrary;
using LetterBoxDClone.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
        private readonly MovieContext _context;
        public List<Movie> MostSeen { get; set; }
        public List<Movie> HighestRated { get; set; }
        public List<CastCrew> ActorsWithHighestRatedMovies { get; set; }

        public IndexModel(ILogger<IndexModel> logger, MovieContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (_context.MovieLeaderboard == null)
            {
                return NotFound();
            }

            var countLeaderboard = await _context.MovieLeaderboard.CountAsync();

            // If leaderboards are empty (first time website has been run), then populate leaderboard tables
            if (countLeaderboard <= 0)
            {
                GetMostSeenMovies();
                GetHighestRatedMovies();
                GetActorsWithHighestRatedMovies();

                for (int i = 0; i < MostSeen.Count; i++)
                {
                    _context.MovieLeaderboard.Add(new MovieLeaderboard("MostSeen", MostSeen[i].MovieId, i));

                }

                for (int i = 0; i < HighestRated.Count; i++)
                {
                    _context.MovieLeaderboard.Add(new MovieLeaderboard("HighestRated", HighestRated[i].MovieId, i));
                }

                for (int i = 0; i < ActorsWithHighestRatedMovies.Count; i++)
                {
                    _context.ActorLeaderboard.Add(new ActorLeaderboard("HighestRatedActors", ActorsWithHighestRatedMovies[i].CastCrewId, i));
                }

                _context.SaveChanges();
            }
            else
            {
                IQueryable<MovieLeaderboard> mostSeenIq = from l in _context.MovieLeaderboard
                                                          where l.LeaderboardCategory.Equals("MostSeen")
                                                          select l;
                MostSeen = mostSeenIq
                    .OrderBy(l => l.LeaderboardCategoryRank)
                    .Select(l => l.Movie)
                    .ToList();

                IQueryable<MovieLeaderboard> highestRatedMovieIq = from l in _context.MovieLeaderboard
                                                                   where l.LeaderboardCategory.Equals("HighestRated")
                                                                   select l;
                HighestRated = highestRatedMovieIq
                    .OrderBy(l => l.LeaderboardCategoryRank)
                    .Select(l => l.Movie)
                    .ToList();

                IQueryable<ActorLeaderboard> highestRatedActorIq = from a in _context.ActorLeaderboard
                                                                   where a.LeaderboardCategory.Equals("HighestRatedActors")
                                                                   select a;
                ActorsWithHighestRatedMovies = highestRatedActorIq
                    .OrderBy(l => l.LeaderboardCategoryRank)
                    .Select(l => l.CastCrew)
                    .ToList();
            }


            //MostSeen = _context.MovieLeaderboard
            //    .Where(l => l.LeaderboardCategory.Equals("MostSeen"))
            //    .OrderBy(l => l.LeaderboardCategoryRank)
            //    .Include(l => l.Movie)
            //    .ToList();
            return Page();
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

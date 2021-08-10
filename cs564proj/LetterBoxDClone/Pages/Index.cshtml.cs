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
using LetterBoxDClone.Models;
using EFCore.BulkExtensions;

namespace LetterBoxDClone.Pages
{
    public class IndexModel : LeaderboardModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly MovieContext _context;

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

                //using var transaction = _context.Database.BeginTransaction();
                //_context.BulkInsert(this.MostSeen);
                //_context.BulkInsert(this.HighestRated);
                //_context.BulkInsert(this.ActorsWithHighestRatedMovies);
                //transaction.Commit();                
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

            return Page();
        }
    }
}

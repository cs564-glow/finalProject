using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary;
using LetterBoxDClone.Models;
using LetterBoxDClone.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LetterBoxDClone.Pages.SummaryInfo
{
    public class SummaryModel : LeaderboardModel
    {
        private readonly MovieContext _context;
        public SummaryModel(MovieContext context)
        {
            _context = context;
        }

        public bool ShouldUpdateLeaderboard { get; set; }
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
        public async Task<IActionResult> OnGetAsync(bool shouldUpdateLeaderboard)
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

            if (shouldUpdateLeaderboard)
            {
                GetMostSeenMovies();
                GetHighestRatedMovies();
                GetActorsWithHighestRatedMovies();

                for (int i = 0; i < MostSeen.Count; i++)
                {
                    _context.MovieLeaderboard.Update(new MovieLeaderboard("MostSeen", MostSeen[i].MovieId, i));

                }

                for (int i = 0; i < HighestRated.Count; i++)
                {
                    _context.MovieLeaderboard.Update(new MovieLeaderboard("HighestRated", HighestRated[i].MovieId, i));
                }

                for (int i = 0; i < ActorsWithHighestRatedMovies.Count; i++)
                {
                    _context.ActorLeaderboard.Update(new ActorLeaderboard("HighestRatedActors", ActorsWithHighestRatedMovies[i].CastCrewId, i));
                }

                _context.SaveChanges();
            }



            return Page();
        }

    }
}

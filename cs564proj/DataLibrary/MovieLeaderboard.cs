using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataLibrary
{
    public class MovieLeaderboard
    {
        public MovieLeaderboard(string leaderboardCategory, int movieId, int leaderboardCategoryRank)
        {
            this.LeaderboardCategory = leaderboardCategory;
            this.MovieId = movieId;
            this.LeaderboardCategoryRank = leaderboardCategoryRank;
        }

        [Key]
        public int LeaderboardId { get; set; }
        public string LeaderboardCategory { get; set; }
        public int MovieId { get; set; }
        public int LeaderboardCategoryRank { get; set; }

        public Movie Movie;
    }
}

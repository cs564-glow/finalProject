using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataLibrary
{
    public class ActorLeaderboard
    {
        public ActorLeaderboard(string leaderboardCategory, string castCrewId, int leaderboardCategoryRank)
        {
            this.LeaderboardCategory = leaderboardCategory;
            this.CastCrewId = castCrewId;
            this.LeaderboardCategoryRank = leaderboardCategoryRank;
        }

        //[Key]
        //public int LeaderboardId { get; set; }
        public string LeaderboardCategory { get; set; }
        public string CastCrewId { get; set; }
        public int LeaderboardCategoryRank { get; set; }

        public CastCrew CastCrew;
    }
}

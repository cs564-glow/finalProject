// TODO: remove redundancy between actors and directors. Implement as we intended from checkpoint 2

using System.ComponentModel.DataAnnotations;

namespace Importer
{
    public class ActsIn
    {
        public int MovieId { get; set; }
        public string CastCrewId { get; set; }
        public int Billing { get; set; }

        public ActsIn(int movieId, string castCrewId, int billing)
        {
            this.MovieId = movieId;
            this.CastCrewId = castCrewId;
            this.Billing = billing;
        }
    }
}
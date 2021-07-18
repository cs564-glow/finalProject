// TODO: remove redundancy between actors and directors. Implement as we intended from checkpoint 2
namespace Importer
{
    public class Directs
    {
        public int MovieId { get; set; }
        public string CastCrewId { get; set; }

        public Directs(int movieId, string castCrewId)
        {
            this.MovieId = movieId;
            this.CastCrewId = castCrewId;
        }
    }
}

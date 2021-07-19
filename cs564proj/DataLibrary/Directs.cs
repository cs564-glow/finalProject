namespace DataLibrary
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

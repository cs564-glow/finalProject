using CsvHelper;

namespace DataLibrary
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

        // https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key#join-entity-type-configuration
        public CastCrew CastCrew { get; set; }
        public Movie Movie { get; set; }
    }
}
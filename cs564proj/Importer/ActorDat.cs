using CsvHelper.Configuration.Attributes;

namespace Importer
{
    public class ActorDat
    {
        [Name("movieID")] public int MovieId { get; set; }
        [Name("actorID")] public string CastCrewId { get; set; }
        [Name("actorName")] public string ActorName { get; set; }
        [Name("ranking")] public int Ranking { get; set; }
    }
}

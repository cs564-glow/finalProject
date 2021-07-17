using CsvHelper.Configuration.Attributes;

// TODO: remove redundancy between actors and directors. Implement as we intended from checkpoint 2
namespace Importer
{
    public class ActsIn
    {
        [Name("movieID")] public int MovieId { get; set; }
        [Name("actorID")] public string CastCrewId { get; set; }
        [Name("actorName")] public string ActorName { get; set; }
        [Name("ranking")] public int Ranking { get; set; }
    }
}
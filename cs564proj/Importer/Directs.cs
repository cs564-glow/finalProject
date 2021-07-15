using CsvHelper.Configuration.Attributes;

// TODO: remove redundancy between actors and directors. Implement as we intended from checkpoint 2
namespace Importer
{
    public class Directs
    {
        [Name("movieID")]
        public int MovieId { get; set; }
        [Name("directorID")]
        public string CastCrewId { get; set; }
        [Name("directorName")]
        public string DirectorName { get; set; }
    }
}

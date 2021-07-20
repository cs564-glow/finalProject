using CsvHelper.Configuration.Attributes;

namespace Importer
{
    public class DirectorDat
    {
        [Name("movieID")]
        public int MovieId { get; set; }
        [Name("directorID")]
        public string CastCrewId { get; set; }
        [Name("directorName")]
        public string DirectorName { get; set; }
    }
}

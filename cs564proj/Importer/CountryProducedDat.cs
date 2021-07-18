using CsvHelper.Configuration.Attributes;

namespace Importer
{
    class CountryProducedDat
    {
        [Name("movieID")] public int MovieId { get; set; }
        [Name("country")] public string Country { get; set; }
        public CountryProducedDat(int movieId, string countryName)
        {
            this.MovieId = movieId;
            this.Country = countryName;
        }
    }
}

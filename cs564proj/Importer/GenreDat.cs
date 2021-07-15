using CsvHelper.Configuration.Attributes;

namespace Importer
{
    /// <summary>
    /// Genre class. Only used for import.
    /// movie_genres.dat does not match our db schema.
    /// </summary>
    public class GenreDat
    { 
        [Name("movieID")]
        public int MovieId { get; set; }
        [Name("genre")]
        public string GenreName { get; set; }
    }
}
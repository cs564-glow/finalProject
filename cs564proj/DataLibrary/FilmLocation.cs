using System.ComponentModel.DataAnnotations;

namespace DataLibrary
{
    public class FilmLocation
    {
        [Key]
        public int FilmLocationId { get; set; }
        public int MovieId { get; set; }
        public int? CountryId { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public Movie Movie { get; set; }
        public Country Country { get; set; }
    }
}
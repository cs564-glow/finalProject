using System.ComponentModel.DataAnnotations;

namespace Importer
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }
}
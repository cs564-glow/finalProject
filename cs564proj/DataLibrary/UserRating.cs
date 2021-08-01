using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DataLibrary
{
    public class UserRating
    {
        [Name("userID")] public long UserId { get; set; }
        [Name("movieID")] public int MovieId { get; set; }
        [Name("rating")] 
        [Range(0, 5)]
        public double Rating { get; set; }
        [Name("timestamp")] public ulong Timestamp { get; set; }
        [Ignore]
        public User User { get; set; }
        [Ignore]
        public Movie Movie { get; set; }
    }
}

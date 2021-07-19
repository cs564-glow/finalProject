using CsvHelper.Configuration.Attributes;

namespace DataLibrary
{
    public class UserRating
    {
        [Name("userID")] public int UserId { get; set; }
        [Name("movieID")] public int MovieId { get; set; }
        [Name("rating")] public double Rating { get; set; }
        [Name("timestamp")] public ulong Timestamp { get; set; }
    }
}

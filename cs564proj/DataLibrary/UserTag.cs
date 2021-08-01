using CsvHelper.Configuration.Attributes;

namespace DataLibrary
{
    public class UserTag
    {
        [Name("userID")] public long UserId { get; set; }
        [Name("movieID")] public int MovieId { get; set; }
        [Name("tagID")] public int TagId { get; set; }
        [Name("timestamp")] public ulong Timestamp { get; set; }
        [Ignore]
        public User User { get; set; }
        [Ignore]
        public Movie Movie { get; set; }
    }
}

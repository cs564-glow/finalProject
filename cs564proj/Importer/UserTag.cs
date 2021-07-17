using CsvHelper.Configuration.Attributes;

namespace Importer
{
    public class UserTag
    {
        [Name("userID")] public int UserId { get; set; }
        [Name("movieID")] public int MovieId { get; set; }
        [Name("tagID")] public int TagId { get; set; }
        [Name("timestamp")] public ulong Timestamp { get; set; }
    }
}

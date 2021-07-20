using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLibrary
{
    public class Movie
    {
        [Name("id"), Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MovieId { get; set; }
        [Name("title")]
        public string Title { get; set; }
        //[Ignore]
        //public string SpanishTitle { get; set; }
        //[Ignore]
        //public string ImdbPictureUrl { get; set; }
        [Name("year")]
        public string Year { get; set; }
        [Ignore]
        public int? CountryId { get; set; }
        [Name("imdbID")]
        public string ImdbId { get; set; }
        [Name("rtID")]
        public string RtId { get; set; }
        [Name("rtAllCriticsRating")]
#nullable enable
        public double? RtAllCriticsRating { get; set; }
        [Name("rtAllCriticsNumReviews")]
        public int? RtAllCriticsNumReviews { get; set; }
#nullable disable
        //[Ignore]
        //public string RtAllNumFresh { get; set; }
        //[Ignore]
        //public string RtAllNumRotten { get; set; }
        //[Ignore]
        //public string RtAllCriticsScore { get; set; }
        //[Ignore]
        //public string RtTopCriticsRating { get; set; }
        //[Ignore]
        //public string RtTopCriticsNumReviews { get; set; }
        //[Ignore]
        //public string RtTopCriticsNumFresh { get; set; }
        //[Ignore]
        //public string RtTopCriticsNumRotten { get; set; }
        //[Ignore]
        //public string RtTopCriticsScore { get; set; }
        //[Ignore]
        //public string RtAudienceRating { get; set; }
        //[Ignore]
        //public string RtAudienceNumRatings { get; set; }
        //[Ignore]
        //public string RtAudienceScore { get; set; }
        //[Ignore]
        //public string RtPictureUrl { get; set; }
    }
}

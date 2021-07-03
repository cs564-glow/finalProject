using System;
using ChoETL;

namespace Importer
{
    class MoviePoco
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string ImdbId { get; set; }
        public string SpanishTitle { get; set; }
        public string ImdbPictureUrl { get; set; }
        public string Year { get; set; }
        public string RtId { get; set; }
        public double RtAllCriticsRating { get; set; }
        public int RtAllCriticsNumReviews { get; set; }
        public int RtAllNumFresh { get; set; }
        public int RtAllNumRotten { get; set; }
        public int RtAllCriticsScore { get; set; }
        public double RtTopCriticsRating { get; set; }
        public int RtTopCriticsNumReviews { get; set; }
        public int RtTopCriticsNumFresh { get; set; }
        public int RtTopCriticsNumRotten { get; set; }
        public int RtTopCriticsScore { get; set; }
        public double RtAudienceRating { get; set; }
        public int RtAudienceNumRatings { get; set; }
        public int RtAudienceScore { get; set; }
        public string RtPictureUrl { get; set; }
    }
}

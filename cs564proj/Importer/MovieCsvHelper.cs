using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace Importer
{
    class MovieCsvHelper
    {
        [Name("id")]
        public int MovieId { get; set; }
        [Name("title")]
        public string Title { get; set; }
        [Name("imdbID")]
        public string ImdbId { get; set; }
        [Ignore]
        public string SpanishTitle { get; set; }
        [Ignore]
        public string ImdbPictureUrl { get; set; }
        [Name("year")]
        public string Year { get; set; }
        [Name("rtID")]
        public string RtId { get; set; }
        [Optional, Name("rtAllCriticsRating")]
        public string RtAllCriticsRating { get; set; }
        [Ignore]
        public string RtAllCriticsNumReviews { get; set; }
        [Ignore]
        public string RtAllNumFresh { get; set; }
        [Ignore]
        public string RtAllNumRotten { get; set; }
        [Ignore]
        public string RtAllCriticsScore { get; set; }
        [Ignore]
        public string RtTopCriticsRating { get; set; }
        [Ignore]
        public string RtTopCriticsNumReviews { get; set; }
        [Ignore]
        public string RtTopCriticsNumFresh { get; set; }
        [Ignore]
        public string RtTopCriticsNumRotten { get; set; }
        [Ignore]
        public string RtTopCriticsScore { get; set; }
        [Ignore]
        public string RtAudienceRating { get; set; }
        [Ignore]
        public string RtAudienceNumRatings { get; set; }
        [Ignore]
        public string RtAudienceScore { get; set; }
        [Ignore]
        public string RtPictureUrl { get; set; }
    }
}

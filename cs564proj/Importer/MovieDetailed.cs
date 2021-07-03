using System;
using FileHelpers;

/// <summary>
/// Represents a movie imported from the HetRec dataset: MovieLens + IMDb/RT
/// https://grouplens.org/datasets/hetrec-2011/
/// </summary>
namespace Importer
{
    [DelimitedRecord("\t"), IgnoreFirst(1)]
    class MovieDetailed
    {
        //[FieldOrder(1), FieldCaption("MovieId")]
        public int MovieId { get; set; }
        //[FieldOrder(2), FieldCaption("Title")]
        public string Title { get; set; }
        //[FieldOrder(3), FieldCaption("ImdbId")]
        public string ImdbId { get; set; }
        //[FieldOrder(4), FieldCaption("SpanishTitle"), FieldHidden]
        [FieldHidden]
        public string SpanishTitle { get; set; }
        //[FieldOrder(5), FieldCaption("ImdbPictureUrl"), FieldHidden]
        [FieldHidden]
        public string ImdbPictureUrl { get; set; }
        //[FieldOrder(6), FieldCaption("Year")]
        public string Year { get; set; }
        //[FieldOrder(7), FieldCaption("RtId")]
        public string RtId { get; set; }
        //[FieldOrder(8), FieldCaption("RtAllCriticsRating")]
        public double RtAllCriticsRating { get; set; }
        //[FieldOrder(9), FieldCaption("RtAllCriticsNumReviews"), FieldHidden]
        [FieldHidden]
        public int RtAllCriticsNumReviews { get; set; }
        //[FieldOrder(10), FieldCaption("RtAllCriticsNumFresh"), FieldHidden]
        [FieldHidden]
        public int RtAllNumFresh { get; set; }
        //[FieldOrder(11), FieldCaption("RtAllCriticsNumRotten"), FieldHidden]
        [FieldHidden]
        public int RtAllNumRotten { get; set; }
        //[FieldOrder(12), FieldCaption("RtAllCriticsScore"), FieldHidden]
        [FieldHidden]
        public int RtAllCriticsScore { get; set; }
        //[FieldOrder(13), FieldCaption("RtTopCriticsRating"), FieldHidden]
        [FieldHidden]
        public double RtTopCriticsRating { get; set; }
        //[FieldOrder(14), FieldCaption("RtTopCriticsNumReviews"), FieldHidden]
        [FieldHidden]
        public int RtTopCriticsNumReviews { get; set; }
        //[FieldOrder(15), FieldCaption("RtTopCriticsNumFresh"), FieldHidden]
        [FieldHidden]
        public int RtTopCriticsNumFresh { get; set; }
        //[FieldOrder(16), FieldCaption("RtTopCriticsNumRotten"), FieldHidden]
        [FieldHidden]
        public int RtTopCriticsNumRotten { get; set; }
        //[FieldOrder(17), FieldCaption("RtTopCriticsScore"), FieldHidden]
        [FieldHidden]
        public int RtTopCriticsScore { get; set; }
        //[FieldOrder(18), FieldCaption("RtAudienceRating"), FieldHidden]
        [FieldHidden]
        public double RtAudienceRating { get; set; }
        //[FieldOrder(19), FieldCaption("RtAudienceNumRatings"), FieldHidden]
        [FieldHidden]
        public int RtAudienceNumRatings { get; set; }
        //[FieldOrder(20), FieldCaption("RtAudienceScore"), FieldHidden]
        [FieldHidden]
        public int RtAudienceScore { get; set; }
        //[FieldOrder(21), FieldCaption("RtPictureUrl"), FieldHidden]
        //[FieldHidden]
        public string RtPictureUrl { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace Importer
{
    [DelimitedRecord(","), IgnoreFirst(1)]
    class Movie
    {
        [FieldOrder(1), FieldCaption("movieId")]
        public int Id { get; set; }
        [FieldOrder(2), FieldCaption("title"), FieldQuoted('"', QuoteMode.OptionalForBoth), FieldConverter(typeof(ConverterRemoveYearFromTitle))]
        public string Title { get; set; }
        [FieldOrder(3), FieldCaption("genres"), FieldDelimiter("|")]
        public string[] GenreArray { get; set; }
    }
}

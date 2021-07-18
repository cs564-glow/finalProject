﻿using CsvHelper.Configuration.Attributes;

namespace Importer
{
    public class FilmLocationDat
    {
        [Name("movieID")] public int MovieId { get; set; }
        [Name("location1")] public string Country { get; set; }
        [Name("location2")] public string State { get; set; }
        [Name("location3")] public string City { get; set; }
        [Name("location4")] public string StreetAddress { get; set; }
    }
}

﻿using System.ComponentModel.DataAnnotations;

namespace Importer
{
    public class FilmLocation
    {
        [Key]
        public int FilmLocationId { get; set; }
        public int MovieId { get; set; }
        public int CountryId { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
    }
}
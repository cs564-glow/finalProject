using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLibrary
{
    //[Index(nameof(GenreName))]
    public class Genre : IEquatable<Genre>
    {
        [Key]
        public int GenreId { get; set; }
        public string GenreName { get; init; }
        public List<MovieGenre> MovieGenreList { get; set; }

        public Genre() { }

        public Genre(string genreName)
        {
            this.GenreName = genreName;
        }

        // https://docs.microsoft.com/en-us/visualstudio/ide/reference/generate-equals-gethashcode-methods?view=vs-2019
        public override int GetHashCode()
        {
            return HashCode.Combine(GenreName);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Genre);
        }

        public bool Equals(Genre other)
        {
            return other != null &&
                   GenreName == other.GenreName;
        }
    }
}
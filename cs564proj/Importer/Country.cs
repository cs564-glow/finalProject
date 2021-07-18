using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Importer
{
    public class Country : IEquatable<Country>
    {
        private static int _id = 0;
        [Key] public int CountryId { get; set; }
        public string Name { get; init; }

        public Country(string name)
        {
            // https://stackoverflow.com/a/154803
            CountryId = Interlocked.Increment(ref _id);
            this.Name = name;
        }

        public bool Equals(Country other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Country) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}

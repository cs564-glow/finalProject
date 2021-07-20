using System;
using System.ComponentModel.DataAnnotations;

namespace DataLibrary
{
    public class CastCrew : IEquatable<CastCrew>
    {
        [Key]
        public string CastCrewId { get; init; }
        public string Name { get; set; }

        public CastCrew(string castCrewId, string name)
        {
            this.CastCrewId = castCrewId;
            this.Name = name;
        }

        public bool Equals(CastCrew other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CastCrewId == other.CastCrewId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CastCrew) obj);
        }

        public override int GetHashCode()
        {
            return (CastCrewId != null ? CastCrewId.GetHashCode() : 0);
            //return CastCrewId.GetHashCode();
        }
    }
}

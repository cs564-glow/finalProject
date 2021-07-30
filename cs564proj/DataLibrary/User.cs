using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLibrary
{
    public class User : IEquatable<User>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long UserId { get; init; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        
        public List<UserRating> userRatings { get; set; }
        

        public User(long userId)
        {
            this.UserId = userId;
        }

        public User()
		{

		}

        public bool Equals(User other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return UserId == other.UserId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }
    }
}
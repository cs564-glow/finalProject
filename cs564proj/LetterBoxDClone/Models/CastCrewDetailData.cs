using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary;

// REMOVE - UNUSED
namespace LetterBoxDClone.Models
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/read-related-data?view=aspnetcore-5.0&amp;tabs=visual-studio#create-a-view-model
    /// </summary>
    public class CastCrewDetailData
    {
        public CastCrew CastCrew { get; set; }
        public IEnumerable<ActsIn> ActsIn { get; set; }
        public IEnumerable<Movie> Movie { get; set; }
    }
}

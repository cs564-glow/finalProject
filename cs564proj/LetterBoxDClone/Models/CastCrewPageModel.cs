using DataLibrary;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace LetterBoxDClone.Models
{
    public class CastCrewPageModel : PageModel
    {
        public SelectList MovieTitleSelectList { get; set; }
        public void PopulateMovieTitleDropDownList(MovieContext _context, object selectedMovie = null)
        {
            var movieQuery = from m in _context.Movie
                             orderby m.Title
                             select m;

            MovieTitleSelectList = new SelectList(movieQuery.AsNoTracking(), "MovieId", "Title", selectedMovie);
        }

        public List<ActsInData> ActsInList;
        //public List<Directs> DirectingCreditList;

        public void PopulateActingCreditList(MovieContext _context, CastCrew CastCrew)
        {
            //var actingCredits = _context.CastCrew
            //    .Include(m => m.ActingRoles)
            //    .ThenInclude(m => m.Movie);
            //var actingRoles = new HashSet<string>(_context.ActsIn.Select(c => c.CastCrewId));
            //ActsInList = new List<ActsInData>();
            //foreach (var actingCredit in actingCredits)
            //{
            //    ActsInList.Add(new ActsInData
            //    {
            //        Movie = actingRoles.Contains()
            //    });
            //}

            var actingRoles = _context.CastCrew
                .Include(m => m.ActingRoles);
            foreach (var role in actingRoles)
            {

            }

        }
    }
}

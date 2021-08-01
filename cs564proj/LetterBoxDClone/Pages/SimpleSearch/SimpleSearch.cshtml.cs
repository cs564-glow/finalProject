using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataLibrary;
using Microsoft.Extensions.Configuration;

namespace LetterBoxDClone.Pages.SimpleSearch
{
    public class SimpleSearchModel : PageModel
    {
        private readonly DataLibrary.MovieContext _context;
        private readonly IConfiguration _configuration;

        public SimpleSearchModel(DataLibrary.MovieContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string CurrentFilter { get; set; }
        public string MovieTitleSort { get; set; }
        public string MovieTitleCurrentSort { get; set; }
        public string CastCrewNameSort { get; set; }
        public string CastCrewNameCurrentSort { get; set; }
        public string UserNameSort { get; set; }
        public string UserNameCurrentSort { get; set; }

        public PaginatedList<Movie> Movie { get; set; }
        public PaginatedList<CastCrew> CastCrew { get; set; }
        public PaginatedList<User> UserList { get; set; }

        public async Task OnGetAsync(string currentFilter, string searchString, int? moviePageIndex, int? castCrewPageIndex, int? userPageIndex)
        {
            if (searchString != null)
            {
                moviePageIndex = 1;
                castCrewPageIndex = 1;
                userPageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Movie> movieIq = from m in _context.Movie
                                        select m;
            IQueryable<CastCrew> castCrewIq = from c in _context.CastCrew
                                              select c;
            IQueryable<User> userIq = from u in _context.User
                                      select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                movieIq = movieIq.Where(m => m.Title.ToUpper().Contains(searchString.ToUpper()));
                castCrewIq = castCrewIq.Where(c => c.Name.ToUpper().Contains(searchString.ToUpper()));
                userIq = userIq.Where(u => u.Username.ToUpper().Contains(searchString.ToUpper()));
            }

            var pageSize = 5; // hardcode simple search page size to 5
            Movie = await PaginatedList<Movie>.CreateAsync(movieIq.AsNoTracking(), moviePageIndex ?? 1, pageSize);
            CastCrew = await PaginatedList<CastCrew>.CreateAsync(castCrewIq.AsNoTracking(), castCrewPageIndex ?? 1, pageSize);
            UserList = await PaginatedList<User>.CreateAsync(userIq.AsNoTracking(), userPageIndex ?? 1, pageSize);
        }
    }
}

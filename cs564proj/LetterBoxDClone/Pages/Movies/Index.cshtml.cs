using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LetterBoxDClone.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly MovieContext _context;
        private readonly IConfiguration _configuration;

        public IndexModel(MovieContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public string NameSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Movie> Movie { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Movie> movieIq = from c in _context.Movie
                                              select c;


            if (!string.IsNullOrEmpty(searchString))
            {
                // case sensitivity: https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/sort-filter-page?view=aspnetcore-5.0#iqueryable-vs-ienumerable
                movieIq = movieIq.Where(c => c.Title.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    movieIq = movieIq.OrderByDescending(c => c.Title);
                    break;
                default:
                    movieIq = movieIq.OrderBy(c => c.Title);
                    break;
            }

            var pageSize = _configuration.GetValue("PageSize", 10);
            Movie = await PaginatedList<Movie>.CreateAsync(
                movieIq.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}

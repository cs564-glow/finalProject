

using System;
using System.Linq;


using System.Threading.Tasks;
using DataLibrary;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LetterBoxDClone.Pages.CastCrewPage
{


    // Sorting: https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/sort-filter-page?view=aspnetcore-5.0#add-sorting
    // Filtering: https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/sort-filter-page?view=aspnetcore-5.0#add-filtering


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

        public PaginatedList<CastCrew> CastCrew { get; set; }

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


            IQueryable<CastCrew> castCrewIq = from c in _context.CastCrew
                                              select c;



            if (!string.IsNullOrEmpty(searchString))
            {
                // case sensitivity: https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/sort-filter-page?view=aspnetcore-5.0#iqueryable-vs-ienumerable
                castCrewIq = castCrewIq.Where(c => c.Name.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    castCrewIq = castCrewIq.OrderByDescending(c => c.Name);
                    break;
                default:
                    castCrewIq = castCrewIq.OrderBy(c => c.Name);
                    break;
            }

            var pageSize = _configuration.GetValue("PageSize", 10);
            CastCrew = await PaginatedList<CastCrew>.CreateAsync(
                castCrewIq.AsNoTracking(), pageIndex ?? 1, pageSize);



        }
    }
}

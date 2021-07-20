using System.Linq;
using System.Threading.Tasks;
using DataLibrary;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LetterBoxDClone.Pages.CastCrewPage
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

        public PaginatedList<CastCrew> CastCrew { get; set; }

        //public IList<CastCrew> CastCrew { get;set; }

        //public async Task OnGetAsync()
        public async Task OnGetAsync(int? pageIndex)
        {
            //CastCrew = await _context.CastCrew.ToListAsync();
            //pageIndex = 1;

            IQueryable<CastCrew> castCrewIq = from c in _context.CastCrew
                                              select c;

            var pageSize = _configuration.GetValue("PageSize", 20);
            CastCrew = await PaginatedList<CastCrew>.CreateAsync(
                castCrewIq.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}

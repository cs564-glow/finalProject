using System.Threading.Tasks;
using DataLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LetterBoxDClone.Pages.CastCrewPage
{
    public class DetailsModel : PageModel
    {
        private readonly MovieContext _context;

        public DetailsModel(MovieContext context)
        {
            _context = context;
        }

        public CastCrew CastCrew { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CastCrew = await _context.CastCrew.FirstOrDefaultAsync(m => m.CastCrewId == id);

            if (CastCrew == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}

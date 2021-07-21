using System.Threading.Tasks;
using DataLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LetterBoxDClone.Pages.CastCrewPage
{
    public class DeleteModel : PageModel
    {
        private readonly MovieContext _context;

        public DeleteModel(MovieContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CastCrew = await _context.CastCrew.FindAsync(id);

            if (CastCrew != null)
            {
                _context.CastCrew.Remove(CastCrew);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

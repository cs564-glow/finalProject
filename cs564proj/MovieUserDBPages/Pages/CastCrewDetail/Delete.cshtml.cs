using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataLibrary;

namespace LetterBoxDClone.Pages.CastCrewDetail
{
    public class DeleteModel : PageModel
    {
        private readonly DataLibrary.MovieContext _context;

        public DeleteModel(DataLibrary.MovieContext context)
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

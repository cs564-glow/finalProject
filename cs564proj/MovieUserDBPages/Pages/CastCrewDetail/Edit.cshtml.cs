using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataLibrary;

namespace LetterBoxDClone.Pages.CastCrewDetail
{
    public class EditModel : PageModel
    {
        private readonly DataLibrary.MovieContext _context;

        public EditModel(DataLibrary.MovieContext context)
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CastCrew).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CastCrewExists(CastCrew.CastCrewId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CastCrewExists(string id)
        {
            return _context.CastCrew.Any(e => e.CastCrewId == id);
        }
    }
}

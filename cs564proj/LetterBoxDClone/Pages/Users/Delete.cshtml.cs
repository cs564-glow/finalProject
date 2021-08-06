using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataLibrary;

namespace LetterBoxDClone.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly DataLibrary.MovieContext _context;

        public DeleteModel(DataLibrary.MovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User UserRecord { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserRecord = await _context.User.FirstOrDefaultAsync(m => m.UserId == id);

            if (UserRecord == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserRecord = await _context.User.FindAsync(id);

            if (UserRecord != null)
            {
                _context.User.Remove(UserRecord);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

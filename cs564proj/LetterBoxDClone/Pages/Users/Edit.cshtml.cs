using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataLibrary;

namespace LetterBoxDClone.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly DataLibrary.MovieContext _context;

        public EditModel(DataLibrary.MovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User1 { get; set; }


        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User1 = await _context.User.FirstOrDefaultAsync(m => m.UserId == id);


            if (User1 == null)
            {
                return NotFound();
            }
            

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // ur = await _context.UserRating.FirstOrDefaultAsync(m => m.MovieId == )
            
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            _context.Attach(User1).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User1.UserId))
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

        private bool UserExists(long id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}

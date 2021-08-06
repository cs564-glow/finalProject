using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataLibrary;

namespace LetterBoxDClone.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly DataLibrary.MovieContext _context;

        public CreateModel(DataLibrary.MovieContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public User UserRecord { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.User.Add(UserRecord);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

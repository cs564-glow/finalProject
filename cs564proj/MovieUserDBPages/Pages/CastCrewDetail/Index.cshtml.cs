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
    public class IndexModel : PageModel
    {
        private readonly DataLibrary.MovieContext _context;

        public IndexModel(DataLibrary.MovieContext context)
        {
            _context = context;
        }

        public IList<CastCrew> CastCrew { get;set; }

        public async Task OnGetAsync()
        {
            CastCrew = await _context.CastCrew.ToListAsync();
        }
    }
}

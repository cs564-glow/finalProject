﻿using System.Threading.Tasks;
using DataLibrary;
using LetterBoxDClone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static LetterBoxDClone.Models.CastCrewDetailData;

namespace LetterBoxDClone.Pages.CastCrewPage
{
    public class DetailsModel : PageModel
    {
        private readonly MovieContext _context;

        public DetailsModel(MovieContext context)
        {
            _context = context;
        }

        //public CastCrewDetailData CastCrewData { get; set; }
        public CastCrew CastCrew { get; set; }
        public ActsIn ActsIn { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // default scaffolding - UNUSED
            //CastCrew = await _context.CastCrew.FirstOrDefaultAsync(m => m.CastCrewId == id);

            //if (CastCrew == null)
            //{
            //return NotFound();
            //}
            // https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/read-related-data?view=aspnetcore-5.0&tabs=visual-studio#scaffold-instructor-pages
            //CastCrewDetailData CastCrewData = new CastCrewDetailData();
            CastCrew = await _context.CastCrew
                .Include(m => m.ActsInRoles)
                .ThenInclude(m => m.Movie)
                .FirstOrDefaultAsync(m => m.CastCrewId == id);

            if (CastCrew == null)
            {
                return NotFound();
            }

            //CastCrewData.ActsIn = CastCrew.ActsInRoles;
            //CastCrewData.Movie = 

            return Page();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataLibrary;
using Microsoft.Extensions.Configuration;

namespace LetterBoxDClone.Pages.Users
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

        public string UsernameSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public PaginatedList<User> UserList { get;set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            UsernameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<User> userIq = from c in _context.User
                                              select c;


            if (!string.IsNullOrEmpty(searchString))
            {
                // case sensitivity: https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/sort-filter-page?view=aspnetcore-5.0#iqueryable-vs-ienumerable
                userIq = userIq.Where(c => c.Username.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    userIq = userIq.OrderByDescending(c => c.Username);
                    break;
                default:
                    userIq = userIq.OrderBy(c => c.Username);
                    break;
            }

            var pageSize = _configuration.GetValue("PageSize", 10);

            UserList = await PaginatedList<User>.CreateAsync(userIq.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}

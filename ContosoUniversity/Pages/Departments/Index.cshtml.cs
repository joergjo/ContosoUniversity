using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Departments
{
    public class IndexModel : PageModel
    {
        private readonly Data.SchoolContext _context;

        public IndexModel(Data.SchoolContext context)
        {
            _context = context;
        }

        public IList<Department> Department { get;set; }

        public async Task OnGetAsync()
        {
            Department = await _context.Departments
                .Include(d => d.Administrator).ToListAsync();
        }
    }
}

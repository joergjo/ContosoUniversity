using ContosoUniversity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Departments
{
    public class DetailsModel : PageModel
    {
        private readonly SchoolContext _context;

        public DetailsModel(SchoolContext context)
        {
            _context = context;
        }

        public Department Department { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Department = await _context.Departments
                .Include(d => d.Administrator).FirstOrDefaultAsync(m => m.Id == id);

            if (Department == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}

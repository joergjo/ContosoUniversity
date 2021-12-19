using ContosoUniversity.Data;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages;

public class About : PageModel
{
    private readonly SchoolContext _context;
    
    public About(SchoolContext context)
    {
        _context = context;
    }
    
    public IList<EnrollmentDateGroup> Students { get; set; }
    
    public async Task OnGetAsync()
    {
        var query = from s in _context.Students
            group s by s.EnrollmentDate
            into dateGroup
            select new EnrollmentDateGroup
            {
                EnrollmentDate = dateGroup.Key,
                StudentCount = dateGroup.Count()
            };

        Students = await query.AsNoTracking().ToListAsync();
    }
}
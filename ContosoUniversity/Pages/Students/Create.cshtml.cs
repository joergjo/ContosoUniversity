using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Students
{
    public class CreateModel : PageModel
    {
        private readonly SchoolContext _context;

        public CreateModel(SchoolContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public Student Student { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var newStudent = new Student();
            if (!await TryUpdateModelAsync(
                    newStudent,
                    "student",
                    s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
            {
                return Page();
            }

            _context.Students.Add(newStudent);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
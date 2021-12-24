using ContosoUniversity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Departments
{
    public class EditModel : PageModel
    {
        private readonly SchoolContext _context;

        public EditModel(SchoolContext context)
        {
            _context = context;
        }

        [BindProperty] 
        public Department Department { get; set; }

        public SelectList InstructorNameList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Department = await _context.Departments
                .Include(d => d.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Department is null)
            {
                return NotFound();
            }

            InstructorNameList = new SelectList(_context.Instructors, "Id", "FullName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var departmentToUpdate = await _context.Departments
                .Include(d => d.Administrator)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (departmentToUpdate is null)
            {
                return HandleDeletedDepartment();
            }

            departmentToUpdate.ConcurrencyToken = Guid.NewGuid();

            _context.Entry(departmentToUpdate).Property(d => d.ConcurrencyToken).OriginalValue =
                Department.ConcurrencyToken;

            if (await TryUpdateModelAsync(
                    departmentToUpdate,
                    "Department",
                    s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Department) exceptionEntry.Entity;
                    var databaseEntry = await exceptionEntry.GetDatabaseValuesAsync();
                    if (databaseEntry is null)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            "Unable to save. The department was deleted by another user.");
                        return Page();
                    }

                    var dbValues = (Department) databaseEntry.ToObject();
                    await SetDbErrorMessage(dbValues, clientValues, _context);

                    Department.ConcurrencyToken = dbValues.ConcurrencyToken;
                    ModelState.Remove($"{nameof(Department)}.{nameof(Department.ConcurrencyToken)}");
                }
            }

            InstructorNameList =
                new SelectList(_context.Instructors, "Id", "FullName", departmentToUpdate.InstructorId);
            return Page();
        }
        
        private IActionResult HandleDeletedDepartment()
        {
            var deletedDepartment = new Department();
            // ModelState.AddModelError(string.Empty,
            //     $"Unable to save. The department was deleted by another user.");
            ModelState.AddModelError(string.Empty,
                $"Unable to save. The department was deleted by another user.");
            InstructorNameList = new SelectList(_context.Instructors, "Id", "FullName", Department.InstructorId);
            return Page();
        }

        private async Task SetDbErrorMessage(Department dbValues, Department clientValues, SchoolContext context)
        {
            if (dbValues.Name != clientValues.Name)
            {
                ModelState.AddModelError(
                    "Department.Name",
                    $"Current value: {dbValues.Name}");
            }
            if (dbValues.Budget != clientValues.Budget)
            {
                ModelState.AddModelError(
                    "Department.Budget",
                    $"Current value: {dbValues.Budget:c}");
            }
            if (dbValues.StartDate != clientValues.StartDate)
            {
                ModelState.AddModelError(
                    "Department.StartDate",
                    $"Current value: {dbValues.StartDate:d}");
            }
            if (dbValues.InstructorId != clientValues.InstructorId)
            {
                Instructor dbInstructor = await _context.Instructors
                    .FindAsync(dbValues.InstructorId);
                ModelState.AddModelError(
                    "Department.InstructorID",
                    $"Current value: {dbInstructor?.FullName}");
            }

            ModelState.AddModelError(string.Empty,
                "The record you attempted to edit "
                + "was modified by another user after you. The "
                + "edit operation was canceled and the current values in the database "
                + "have been displayed. If you still want to edit this record, click "
                + "the Save button again.");
        }
    }
}
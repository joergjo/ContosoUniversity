using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Instructors
{
    public class CreateModel : InstructorCoursesPageModel
    {
        private readonly SchoolContext _context;
        private readonly ILogger _logger;

        public CreateModel(SchoolContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            var instructor = new Instructor
            {
                Courses = new List<Course>()
            };
            PopulateAssignedCourseData(_context, instructor);
            return Page();
        }

        public Instructor Instructor { get; set; }

        public async Task<IActionResult> OnPostAsync(string[] selectedCourses)
        {
            var newInstructor = new Instructor();
            if (selectedCourses.Length > 0)
            {
                newInstructor.Courses = new List<Course>();
                await _context.Courses.LoadAsync();
            }

            foreach (var course in selectedCourses)
            {
                var courseToAdd = await _context.Courses.FindAsync(int.Parse(course));
                if (courseToAdd is not null)
                {
                    newInstructor.Courses.Add(courseToAdd);
                }
                else
                {
                    _logger.LogWarning("Course {course} not found", course);
                }
            }

            try
            {
                if (!await TryUpdateModelAsync(
                        newInstructor,
                        "Instructor",
                        i => i.FirstMidName, i => i.LastName,
                        i => i.HireDate, i => i.OfficeAssignment))
                {
                    return Page();
                }

                _context.Instructors.Add(newInstructor);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating instructor");
            }

            PopulateAssignedCourseData(_context, newInstructor);
            return Page();
        }
    }
}
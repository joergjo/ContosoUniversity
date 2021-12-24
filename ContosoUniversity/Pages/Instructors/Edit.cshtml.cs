using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Instructors
{
    public class EditModel : InstructorCoursesPageModel
    {
        private readonly SchoolContext _context;

        public EditModel(SchoolContext context)
        {
            _context = context;
        }

        public Instructor Instructor { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Instructor is null)
            {
                return NotFound();
            }

            PopulateAssignedCourseData(_context, Instructor);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedCourses)
        {
            if (id is null)
            {
                return NotFound();
            }

            var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (instructorToUpdate is null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync(
                    instructorToUpdate,
                    "Instructor",
                    i => i.FirstMidName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment))
            {
                if (string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;
                }

                UpdateInstructorCourses(selectedCourses, instructorToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            UpdateInstructorCourses(selectedCourses, instructorToUpdate);
            PopulateAssignedCourseData(_context, instructorToUpdate);
            return Page();
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructor)
        {
            if (selectedCourses == null)
            {
                instructor.Courses = new List<Course>();
                return;
            }

            var selectedCoursesSet = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.Id));
            foreach (var course in _context.Courses)
            {
                if (selectedCoursesSet.Contains(course.Id.ToString()))
                {
                    if (!instructorCourses.Contains(course.Id))
                    {
                        instructor.Courses.Add(course);
                    }
                }
                else
                {
                    if (!instructorCourses.Contains(course.Id))
                    {
                        continue;
                    }

                    var courseToRemove = instructor.Courses.Single(c => c.Id == course.Id);
                    instructor.Courses.Remove(courseToRemove);
                }
            }
        }
    }
}
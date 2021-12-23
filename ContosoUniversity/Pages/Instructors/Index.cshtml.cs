using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;
using ContosoUniversity.Pages.Students;

namespace ContosoUniversity.Pages.Instructors
{
    public class IndexModel : PageModel
    {
        private readonly SchoolContext _context;

        public IndexModel(SchoolContext context)
        {
            _context = context;
        }

        public InstructorIndexData InstructorData { get; set; }
        public int InstructorId { get; set; }
        public int CourseId { get; set; }

        public async Task OnGetAsync(int? id, int? courseId)
        {
            InstructorData = new InstructorIndexData
            {
                Instructors = await _context.Instructors
                    .Include(i => i.OfficeAssignment)
                    .Include(i => i.Courses)
                    .ThenInclude(c => c.Department)
                    .OrderBy(i => i.LastName)
                    .ToListAsync()
            };

            if (id is not null)
            {
                InstructorId = id.Value;
                var instructor = InstructorData.Instructors.Single(i => i.Id == id.Value);
                InstructorData.Courses = instructor.Courses;
            }

            if (courseId is not null)
            {
                CourseId = courseId.Value;
                var selectedCourse = InstructorData.Courses.Single(x => x.Id == courseId);
                await _context.Entry(selectedCourse).Collection(x => x.Enrollments).LoadAsync();
                foreach (var enrollment in selectedCourse.Enrollments)
                {
                    await _context.Entry(enrollment).Reference(x => x.Student).LoadAsync();
                }
                InstructorData.Enrollments = selectedCourse.Enrollments;
            }
        }
    }
}

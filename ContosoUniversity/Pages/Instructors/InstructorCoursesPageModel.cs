using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoUniversity.Pages.Instructors;

public class InstructorCoursesPageModel : PageModel
{
    public List<AssignedCourseData> AssignedCourseDataList { get; set; } = new();

    protected void PopulateAssignedCourseData(SchoolContext context, Instructor instructor)
    {
        var allCourses = context.Courses;
        var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.Id));
        foreach (var course in allCourses)
        {
            AssignedCourseDataList.Add(new AssignedCourseData
            {
                CourseId = course.Id,
                Title = course.Title,
                Assigned = instructorCourses.Contains(course.Id)
            });
        }
    }
}
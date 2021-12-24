using ContosoUniversity.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Courses;

public class DepartmentNamePageModel : PageModel
{
    public SelectList DepartmentNameList { get; set; }

    protected void PopulateDepartmentsDropDownList(SchoolContext context, object selectedDepartment = null)
    {
        var departmentsQuery = from d in context.Departments
                               orderby d.Name
                               select d;

        DepartmentNameList = new SelectList(departmentsQuery.AsNoTracking(),
                        "Id", "Name", selectedDepartment);
    }
}
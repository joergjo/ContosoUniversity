using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models;

public class Course
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string Title { get; set; }
    public int Credits { get; set; }    
    
    public ICollection<Enrollment> Enrollments { get; set; }
}
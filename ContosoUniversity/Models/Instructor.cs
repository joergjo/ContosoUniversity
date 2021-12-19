using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models;

public class Instructor
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Last Name")]
    [StringLength(50)]
    public string LastName { get; set; }
 
    [Required]
    [Display(Name = "First Name")]
    [StringLength(50)]
    public string FirstMidName { get; set; }
    
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Display(Name = "Hire Date")]
    public DateTime HireDate { get; set; }
    
    [Display(Name = "Full Name")] 
    public string FullName => $"{LastName}, {FirstMidName}";

    public ICollection<Course> Courses { get; set; }
    public OfficeAssignment OfficeAssignment { get; set; }
}
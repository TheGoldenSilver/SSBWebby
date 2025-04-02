using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseApplication.Models
{
    public class Course
    {
        [Key]
        [Display(Name = "Course ID")]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        [Display(Name = "Course Name")]
        public required string CourseName { get; set; }

        [Required]
        [MaxLength(1000)]
        [Column(TypeName = "VARCHAR(1000)")]
        [Display(Name = "Course Description")]
        public required string CourseDescription { get; set; }
    }
}

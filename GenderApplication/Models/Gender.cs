using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenderApplication.Models
{
    public class Gender
    {
        [Key]
        [Display(Name = "Gender ID")]
        public int GenderId { get; set; }

        [Required]
        [MaxLength(6)]
        [Column(TypeName = "VARCHAR(6)")]
        [Display(Name = "Gender Name")]
        public required string GenderName { get; set; }
    }
}

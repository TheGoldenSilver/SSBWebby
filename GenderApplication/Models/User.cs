using System.ComponentModel.DataAnnotations;

namespace GenderApplication.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        [StringLength(50)]
        public string Province { get; set; }

        [Required]
        [StringLength(50)]
        public string Education { get; set; }


        [Required]
        [StringLength(50)]
        public string WorkExperience { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string CareerOpportunities { get; set; }

        [StringLength(100)]
        public string TrainingCourse { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = false;

        public string? EmailVerificationToken { get; set; }

        public DateTime? EmailVerificationTokenExpiry { get; set; }

        public bool IsEmailVerified { get; set; } = false;
    }
}
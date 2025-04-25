using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenderApplication.Models
{
    public class AssessmentResponse
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserEmail { get; set; } = string.Empty;
        [Required]
        public required string SoftSkills { get; set; } = string.Empty; // JSON array of int
        [Required]
        public required string TechSkills { get; set; } = string.Empty; // JSON array of int
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}

using Microsoft.EntityFrameworkCore;
using GenderApplication.Models;

namespace GenderApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Gender> Genders { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GenderApplication.Models.AssessmentResponse> AssessmentResponses { get; set; }
    }
}

using CourseApplication.Models;
using GenderApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace GenderApplication.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> Options): base(Options)
        {
            
        }
        
        public DbSet<Gender> Genders { get; set; }

        public DbSet<Course> Courses { get; set; }
    }
}

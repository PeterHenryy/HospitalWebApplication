using HospitalWebApplication.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalWebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed roles
            builder.Entity<AppRole>().HasData(
                new AppRole { Id = 1, Name = "Patient", NormalizedName = "PATIENT" },
                new AppRole { Id = 2, Name = "Doctor", NormalizedName = "DOCTOR" },
                new AppRole { Id = 3, Name = "Admin", NormalizedName = "ADMIN" }
            );
        }
    }
}

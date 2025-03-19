using HospitalWebApplication.Models;
using HospitalWebApplication.Models.Doctors;
using HospitalWebApplication.Models.Identity;
using HospitalWebApplication.Models.Patients;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace HospitalWebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<PROMIS10> PROMIS10s { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillItem> BillItems { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
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

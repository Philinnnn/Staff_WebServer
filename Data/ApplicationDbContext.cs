using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Models;

namespace Staff_WebServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<PensionFund> PensionFunds { get; set; }
        public DbSet<PositionCategory> PositionCategories { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<OrderType> OrderTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<StaffSchedule> StaffSchedules { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StaffSchedule>()
                .HasKey(ss => new { ss.DepartmentId, ss.PositionId });


            builder.Entity<Department>()
                .HasOne(d => d.Head)
                .WithMany()
                .HasForeignKey(d => d.HeadEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Employee>()
                .HasOne(e => e.Nationality)
                .WithMany(n => n.Employees)
                .HasForeignKey(e => e.NationalityId);

            builder.Entity<Employee>()
                .HasOne(e => e.Education)
                .WithMany(ed => ed.Employees)
                .HasForeignKey(e => e.EducationId);

            builder.Entity<Employee>()
                .HasOne(e => e.PensionFund)
                .WithMany(pf => pf.Employees)
                .HasForeignKey(e => e.PensionFundId);

            builder.Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId);

            builder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId);

            builder.Entity<Position>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Positions)
                .HasForeignKey(p => p.CategoryId);

            builder.Entity<Order>()
                .HasOne(o => o.OrderType)
                .WithMany(ot => ot.Orders)
                .HasForeignKey(o => o.OrderTypeId);

            builder.Entity<Order>()
                .HasOne(o => o.Employee)
                .WithMany()
                .HasForeignKey(o => o.EmployeeId);
        }
    }
}

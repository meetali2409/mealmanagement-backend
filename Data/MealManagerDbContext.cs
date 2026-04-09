using Microsoft.EntityFrameworkCore;
using MealManagement.Models;

namespace MealManagement.Data
{
    public class MealManagerDbContext : DbContext
    {
        public MealManagerDbContext(DbContextOptions<MealManagerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<MealType> MealTypes { get; set; }
        public DbSet<MealRecord> MealRecords { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MealType>()
                .Property(m => m.FixedPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<MealRecord>()
                .HasIndex(m => new { m.EmployeeId, m.MealTypeId, m.MealDate });


            modelBuilder.Entity<MealRecord>()
                .HasOne(m => m.Employee)
                .WithMany()
                .HasForeignKey(m => m.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MealRecord>()
                .HasOne(m => m.MealType)
                .WithMany()
                .HasForeignKey(m => m.MealTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MealRecord>()
                .HasOne(m => m.FoodItem)
                .WithMany()
                .HasForeignKey(m => m.FoodId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
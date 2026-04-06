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
        }
    }
}
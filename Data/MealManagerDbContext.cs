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
            // ✅ Price precision fix
            modelBuilder.Entity<MealType>()
                .Property(m => m.FixedPrice)
                .HasPrecision(10, 2);

            // ✅ Prevent duplicate meals (same user + meal + date)
            modelBuilder.Entity<MealRecord>()
                .HasIndex(m => new { m.EmployeeId, m.MealTypeId, m.MealDate });

            // ❌ IMPORTANT: FoodItem relation REMOVE kar diya
            // (Yehi tumhara error ka reason tha)
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MealManagement.Data;
using MealManagement.Models;

namespace MealManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly MealManagerDbContext _context;

        public MealController(MealManagerDbContext context)
        {
            _context = context;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddMeal(AddMealDto request)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var existingMeal = await _context.MealRecords.AnyAsync(m =>
                m.EmployeeId == request.EmployeeId &&
                m.MealTypeId == request.MealTypeId &&
                m.MealDate >= today &&
                m.MealDate < tomorrow);

            if (existingMeal)
            {
                return BadRequest(new { message = "Meal already taken today" });
            }

            var meal = new MealRecord
            {
                EmployeeId = request.EmployeeId,
                MealTypeId = request.MealTypeId,
                MealDate = DateTime.UtcNow
            };

            _context.MealRecords.Add(meal);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Meal Added Successfully" });
        }

        [HttpGet("CheckTodayMeal/{employeeId}/{mealTypeId}")]
        public IActionResult CheckTodayMeal(int employeeId, int mealTypeId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            bool exists = _context.MealRecords.Any(r =>
                r.EmployeeId == employeeId &&
                r.MealTypeId == mealTypeId &&
                r.MealDate >= today &&
                r.MealDate < tomorrow);

            return Ok(exists);
        }

        [HttpGet("TodayTotalPlates")]
        public IActionResult TodayTotalPlates()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var total = _context.MealRecords
                .Count(r => r.MealDate >= today && r.MealDate < tomorrow);

            return Ok(total);
        }

        [HttpGet("TodayTotalAmount")]
        public IActionResult TodayTotalAmount()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var records = _context.MealRecords
                .Include(r => r.MealType)
                .Where(r => r.MealDate >= today && r.MealDate < tomorrow)
                .ToList();

            var total = records.Sum(r => r.MealType != null ? r.MealType.FixedPrice : 0);

            return Ok(total);
        }

        [HttpGet("History")]
        public IActionResult GetHistory(DateTime? fromDate, DateTime? toDate, string? name, int? mealTypeId)
        {
            var query = _context.MealRecords
                .Include(r => r.MealType)
                .Include(r => r.Employee)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(r => r.MealDate >= fromDate.Value.ToUniversalTime());

            if (toDate.HasValue)
                query = query.Where(r => r.MealDate < toDate.Value.AddDays(1).ToUniversalTime());

            if (!string.IsNullOrEmpty(name))
                query = query.Where(r => r.Employee.FullName.Contains(name));

            if (mealTypeId.HasValue)
                query = query.Where(r => r.MealTypeId == mealTypeId.Value);

            var data = query.Select(r => new
            {
                fullName = r.Employee.FullName,
                mealDate = r.MealDate,
                mealName = r.MealType != null ? r.MealType.MealName : "",
                fixedPrice = r.MealType != null ? r.MealType.FixedPrice : 0
            }).ToList();

            var totalAmount = data.Sum(x => x.fixedPrice);

            return Ok(new
            {
                records = data,
                totalAmount = totalAmount
            });
        }
    }
}
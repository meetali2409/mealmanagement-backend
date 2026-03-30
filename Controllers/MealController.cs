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

            var exists = await _context.MealRecords.AnyAsync(m =>
                m.EmployeeId == request.EmployeeId &&
                m.MealTypeId == request.MealTypeId &&
                m.FoodId == request.FoodId &&
                m.MealDate >= today &&
                m.MealDate < tomorrow);

            if (exists)
            {
                return BadRequest(new { message = "Meal already taken today" });
            }

            var meal = new MealRecord
            {
                EmployeeId = request.EmployeeId,
                MealTypeId = request.MealTypeId,
                FoodId = request.FoodId, 
                MealDate = DateTime.UtcNow
            };

            _context.MealRecords.Add(meal);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Meal Added Successfully" });
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
            try
            {
                var query = _context.MealRecords
                    .Include(r => r.MealType)
                    .Include(r => r.Employee)
                    .AsQueryable();

                if (fromDate.HasValue)
                {
                    var from = fromDate.Value.Date;
                    query = query.Where(r => r.MealDate >= from);
                }

                if (toDate.HasValue)
                {
                    var to = toDate.Value.Date.AddDays(1);
                    query = query.Where(r => r.MealDate < to);
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(r => r.Employee.FullName.Contains(name));
                }

                if (mealTypeId.HasValue)
                {
                    query = query.Where(r => r.MealTypeId == mealTypeId.Value);
                }

                var data = query.Select(r => new
                {
                    fullName = r.Employee != null ? r.Employee.FullName : "",
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }
    }
}
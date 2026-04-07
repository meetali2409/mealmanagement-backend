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

        [HttpPost("AddBulk")]
        public async Task<IActionResult> AddBulk(AddBulkMealDto dto)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var tomorrow = today.AddDays(1);

                var exists = await _context.MealRecords.AnyAsync(m =>
                    m.EmployeeId == dto.EmployeeId &&
                    m.MealTypeId == dto.MealTypeId &&
                    m.MealDate >= today &&
                    m.MealDate < tomorrow);

                if (exists)
                {
                    return BadRequest(new { message = "Meal already taken today" });
                }

                foreach (var foodId in dto.FoodIds)
                {
                    var foodExists = await _context.FoodItems.AnyAsync(f => f.FoodId == foodId);

                    if (!foodExists)
                    {
                        return BadRequest($"Invalid FoodId: {foodId}");
                    }

                    _context.MealRecords.Add(new MealRecord
                    {
                        EmployeeId = dto.EmployeeId,
                        MealTypeId = dto.MealTypeId,
                        FoodId = foodId,
                        MealDate = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Meal Added Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("TodayTotalPlates")]
        public IActionResult TodayTotalPlates()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var total = _context.MealRecords
                .Where(r => r.MealDate >= today && r.MealDate < tomorrow)
                .Select(r => new { r.EmployeeId, r.MealTypeId })
                .Distinct()
                .Count();

            return Ok(total);
        }

        [HttpGet("TodayTotalAmount")]
        public IActionResult TodayTotalAmount()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var total = _context.MealRecords
                .Include(r => r.MealType)
                .Where(r => r.MealDate >= today && r.MealDate < tomorrow)
                .GroupBy(r => new { r.EmployeeId, r.MealTypeId })
                .Select(g => g.First().MealType.FixedPrice)
                .Sum();

            return Ok(total);
        }

        [HttpGet("History/{employeeId}")]
        public IActionResult GetHistory(int employeeId)
        {
            var records = _context.MealRecords
                .Include(r => r.MealType)
                .Include(r => r.FoodItem)
                .Where(r => r.EmployeeId == employeeId) 
                .OrderByDescending(r => r.MealDate)
                .Select(r => new
                {
                    mealDate = r.MealDate,
                    mealName = r.MealType.MealName,
                    foodNames = new List<string> { r.FoodItem.FoodName },
                    fixedPrice = r.MealType.FixedPrice
                })
                .ToList();

            var totalAmount = records.Sum(r => r.fixedPrice);

            return Ok(new { records, totalAmount });
        }
    }
}
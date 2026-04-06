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

        // ✅ ADD BULK (MAIN FIX)
        [HttpPost("AddBulk")]
        public async Task<IActionResult> AddBulk(AddBulkMealDto dto)
        {
            var today = DateTime.Now.Date; // ✅ FIXED
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
                _context.MealRecords.Add(new MealRecord
                {
                    EmployeeId = dto.EmployeeId,
                    MealTypeId = dto.MealTypeId,
                    FoodId = foodId,
                    MealDate = DateTime.Now // ✅ FIXED
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Meal Added Successfully" });
        }

        // ✅ PLATE COUNT FIX
        [HttpGet("TodayTotalPlates")]
        public IActionResult TodayTotalPlates()
        {
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);

            var total = _context.MealRecords
                .Where(r => r.MealDate >= today && r.MealDate < tomorrow)
                .Select(r => new { r.EmployeeId, r.MealTypeId })
                .Distinct()
                .Count();

            return Ok(total);
        }

        // ✅ AMOUNT FIX
        [HttpGet("TodayTotalAmount")]
        public IActionResult TodayTotalAmount()
        {
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);

            var total = _context.MealRecords
                .Include(r => r.MealType)
                .Where(r => r.MealDate >= today && r.MealDate < tomorrow)
                .GroupBy(r => new { r.EmployeeId, r.MealTypeId })
                .Select(g => g.First().MealType.FixedPrice)
                .Sum();

            return Ok(total);
        }
    }
}
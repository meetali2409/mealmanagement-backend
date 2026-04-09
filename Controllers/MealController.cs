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
                    return BadRequest(new { message = "Meal already taken today" });

                foreach (var foodId in dto.FoodIds)
                {
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
                return StatusCode(500, ex.Message);
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

        [HttpGet("History")]
        public IActionResult GetAllHistory(
            DateTime? fromDate,
            DateTime? toDate,
            string? name,
            int? mealTypeId)
        {
            var query = _context.MealRecords
                .Include(r => r.Employee)
                .Include(r => r.MealType)
                .Include(r => r.FoodItem)
                .AsQueryable();

            if (fromDate.HasValue)
            {
                var fromUtc = DateTime.SpecifyKind(fromDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(r => r.MealDate >= fromUtc);
            }

            if (toDate.HasValue)
            {
                var toUtc = DateTime.SpecifyKind(toDate.Value.Date.AddDays(1), DateTimeKind.Utc);
                query = query.Where(r => r.MealDate < toUtc);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r =>
                    r.Employee != null &&
                    r.Employee.FullName.Contains(name) 
                );
            }

            if (mealTypeId.HasValue)
            {
                query = query.Where(r => r.MealTypeId == mealTypeId.Value);
            }

            var data = query.ToList();

            var grouped = data
                .GroupBy(r => new
                {
                    Date = r.MealDate.Date,
                    r.EmployeeId,
                    r.MealTypeId
                })
                .Select(g => new
                {
                    employeeId = g.First().EmployeeId,
                    fullName = g.First().Employee?.FullName ?? "",
                    mealDate = g.First().MealDate,
                    mealName = g.First().MealType?.MealName ?? "",

                    foodNames = g
                        .Where(x => x.FoodItem != null)
                        .Select(x => x.FoodItem.FoodName)
                        .Distinct()
                        .ToList(),

                    fixedPrice = g.First().MealType?.FixedPrice ?? 0,
                    mealTypeId = g.First().MealTypeId
                })
                .OrderByDescending(x => x.mealDate)
                .ToList();

            var totalAmount = grouped.Sum(x => x.fixedPrice);

            return Ok(new { records = grouped, totalAmount });
        }
        [HttpGet("MyHistory/{employeeId}")]
        public IActionResult GetMyHistory(int employeeId)
        {
            var data = _context.MealRecords
                .Include(r => r.MealType)
                .Include(r => r.FoodItem)
                .Where(r => r.EmployeeId == employeeId)
                .ToList();

            var grouped = data
                .GroupBy(r => new
                {
                    Date = r.MealDate.Date,
                    r.EmployeeId,
                    r.MealTypeId
                })
                .Select(g => new
                {
                    employeeId = g.First().EmployeeId,
                    mealDate = g.First().MealDate,
                    mealName = g.First().MealType?.MealName ?? "",

                    foodNames = g
                        .Where(x => x.FoodItem != null)
                        .Select(x => x.FoodItem.FoodName)
                        .Distinct()
                        .ToList(),

                    fixedPrice = g.First().MealType?.FixedPrice ?? 0
                })
                .OrderByDescending(x => x.mealDate)
                .ToList();

            var totalAmount = grouped.Sum(x => x.fixedPrice);

            return Ok(new { records = grouped, totalAmount });
        }
        [HttpDelete("Delete")]
        public IActionResult DeleteMeal(int employeeId, int mealTypeId, DateTime mealDate)
        {
            var records = _context.MealRecords
                .Where(r => r.EmployeeId == employeeId &&
                            r.MealTypeId == mealTypeId &&
                            r.MealDate.Date == mealDate.Date)
                .ToList();

            if (!records.Any())
                return NotFound("No record");

            _context.MealRecords.RemoveRange(records);
            _context.SaveChanges();

            return Ok("Deleted");
        }
    }
}
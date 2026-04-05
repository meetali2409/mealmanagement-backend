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

            var meal = new MealRecord
            {
                EmployeeId = dto.EmployeeId,
                MealTypeId = dto.MealTypeId,
                MealDate = DateTime.UtcNow,

   
                FoodName = string.Join(",", dto.FoodIds)
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
                .Where(r => r.MealDate >= today && r.MealDate < tomorrow)
                .Select(r => new { r.EmployeeId, r.MealTypeId })
                .Distinct()
                .Count();

            return Ok(total);
        }

        // ✅ TOTAL AMOUNT (FIXED PRICE)
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

                var data = query
                    .AsEnumerable()
                    .GroupBy(r => new
                    {
                        r.Employee.FullName,
                        Date = r.MealDate.Date,
                        r.MealType.MealName,
                        r.MealType.FixedPrice,
                        r.EmployeeId,
                        r.MealTypeId
                    })
                    .Select(g => new
                    {
                        employeeId = g.Key.EmployeeId,
                        mealTypeId = g.Key.MealTypeId,
                        fullName = g.Key.FullName,
                        mealDate = g.Key.Date,
                        mealName = g.Key.MealName,

                        foodNames = g
                            .SelectMany(x => x.FoodName
                                .Split(',', StringSplitOptions.RemoveEmptyEntries))
                            .ToList(),

                        fixedPrice = g.Key.FixedPrice
                    })
                    .ToList();

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

        [HttpDelete("Delete/{employeeId}/{mealTypeId}")]
        public async Task<IActionResult> Delete(int employeeId, int mealTypeId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var records = await _context.MealRecords
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.MealTypeId == mealTypeId &&
                    x.MealDate >= today &&
                    x.MealDate < tomorrow)
                .ToListAsync();

            if (!records.Any())
                return NotFound();

            _context.MealRecords.RemoveRange(records);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted Successfully" });
        }

        // ✅ EDIT (UPDATE FOOD ITEMS)
        [HttpPut("Update")]
        public async Task<IActionResult> Update(AddBulkMealDto dto)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var record = await _context.MealRecords
                .FirstOrDefaultAsync(x =>
                    x.EmployeeId == dto.EmployeeId &&
                    x.MealTypeId == dto.MealTypeId &&
                    x.MealDate >= today &&
                    x.MealDate < tomorrow);

            if (record == null)
                return NotFound();

            record.FoodName = string.Join(",", dto.FoodIds);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Updated Successfully" });
        }
    }
}
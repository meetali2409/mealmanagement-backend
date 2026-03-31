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

            var total = _context.MealRecords
                .Include(r => r.MealType)
                .Where(r => r.MealDate >= today && r.MealDate < tomorrow)
                .Sum(r => r.MealType != null ? r.MealType.FixedPrice : 0);

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
                    .Include(r => r.FoodItem)
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
                    id = r.Id,
                    fullName = r.Employee != null ? r.Employee.FullName : "",
                    mealDate = r.MealDate,
                    mealName = r.MealType != null ? r.MealType.MealName : "",
                    foodName = r.FoodItem != null ? r.FoodItem.FoodName : "",
                    fixedPrice = r.MealType != null ? r.MealType.FixedPrice : 0,
                    employeeId = r.EmployeeId,
                    mealTypeId = r.MealTypeId
                }).ToList();
                var totalAmount = data
                    .GroupBy(x => new
                    {
                        x.employeeId,
                        Date = x.mealDate.Date,
                        x.mealTypeId
                    })
                    .Sum(g => g.First().fixedPrice);

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
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            var meal = await _context.MealRecords.FindAsync(id);

            if (meal == null)
                return NotFound("Meal not found");

            _context.MealRecords.Remove(meal);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Meal deleted successfully" });
        }
        [HttpDelete("DeleteByGroup")]
        public async Task<IActionResult> DeleteByGroup(int employeeId, int mealTypeId, DateTime date)
        {
            var nextDay = date.Date.AddDays(1);

            var meals = _context.MealRecords
                .Where(m =>
                    m.EmployeeId == employeeId &&
                    m.MealTypeId == mealTypeId &&
                    m.MealDate >= date.Date &&
                    m.MealDate < nextDay
                );

            _context.MealRecords.RemoveRange(meals);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Meal group deleted" });
        }
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateMeal(int id, AddMealDto request)
        {
            var meal = await _context.MealRecords.FindAsync(id);

            if (meal == null)
                return NotFound("Meal not found");

            meal.MealTypeId = request.MealTypeId;
            meal.FoodId = request.FoodId;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Meal Updated Successfully" });
        }
    }
}
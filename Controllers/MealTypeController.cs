using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MealManagement.Data;
using MealManagement.Models;

namespace MealManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealTypeController : ControllerBase
    {
        private readonly MealManagerDbContext _context;

        public MealTypeController(MealManagerDbContext context)
        {
            _context = context;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddMealType(MealTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MealName))
                return BadRequest("Meal name is required");

            if (dto.FixedPrice <= 0)
                return BadRequest("Price must be greater than 0");

            var mealType = new MealType
            {
                MealName = dto.MealName,
                FixedPrice = dto.FixedPrice
            };

            await _context.MealTypes.AddAsync(mealType);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Meal type added successfully",
                data = mealType
            });
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllMealTypes()
        {
            var meals = await _context.MealTypes.ToListAsync();

            return Ok(meals); // frontend already expects array ✔
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateMealType(int id, MealTypeDto dto)
        {
            var meal = await _context.MealTypes.FindAsync(id);

            if (meal == null)
                return NotFound("Meal type not found");

            if (string.IsNullOrWhiteSpace(dto.MealName))
                return BadRequest("Meal name is required");

            if (dto.FixedPrice <= 0)
                return BadRequest("Price must be greater than 0");

            meal.MealName = dto.MealName;
            meal.FixedPrice = dto.FixedPrice;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Meal type updated successfully",
                data = meal
            });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteMealType(int id)
        {
            var meal = await _context.MealTypes.FindAsync(id);

            if (meal == null)
                return NotFound("Meal type not found");

            _context.MealTypes.Remove(meal);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Meal type deleted successfully"
            });
        }
    }
}
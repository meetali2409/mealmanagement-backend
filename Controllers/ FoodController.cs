using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MealManagement.Data;
using MealManagement.Models;

namespace MealManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly MealManagerDbContext _context;

        public FoodController(MealManagerDbContext context)
        {
            _context = context;
        }
        [HttpPost("Add")]
        public async Task<IActionResult> AddFood(FoodDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FoodName))
                return BadRequest("Food name is required");

            var mealType = await _context.MealTypes.FindAsync(dto.MealTypeId);

            if (mealType == null)
                return BadRequest("Invalid Meal Type");

            var food = new FoodItem
            {
                FoodName = dto.FoodName,
                MealTypeId = dto.MealTypeId
            };

            await _context.FoodItems.AddAsync(food);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Food added successfully",
                data = food
            });
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllFoods()
        {
            var foods = await _context.FoodItems.ToListAsync();

            return Ok(new
            {
                message = "Food list fetched",
                data = foods
            });
        }
        [HttpGet("ByMeal/{mealTypeId}")]
        public async Task<IActionResult> GetFoodByMeal(int mealTypeId)
        {
            var foods = await _context.FoodItems
                .Where(f => f.MealTypeId == mealTypeId)
                .ToListAsync();

            return Ok(foods);
        }
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateFood(int id, FoodDto dto)
        {
            var food = await _context.FoodItems.FindAsync(id);

            if (food == null)
                return NotFound("Food not found");

            if (string.IsNullOrWhiteSpace(dto.FoodName))
                return BadRequest("Food name is required");

            food.FoodName = dto.FoodName;
            food.MealTypeId = dto.MealTypeId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Food updated successfully",
                data = food
            });
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var food = await _context.FoodItems.FindAsync(id);

            if (food == null)
                return NotFound("Food not found");

            _context.FoodItems.Remove(food);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Food deleted successfully"
            });
        }
    }
}
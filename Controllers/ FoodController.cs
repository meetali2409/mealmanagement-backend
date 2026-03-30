using Microsoft.AspNetCore.Mvc;
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
        public IActionResult AddFood(FoodDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FoodName))
                return BadRequest("Food name is required");

            var food = new FoodItem
            {
                FoodName = dto.FoodName,
            };

            _context.FoodItems.Add(food);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Food added successfully",
                data = food
            });
        }

        [HttpGet("All")]
        public IActionResult GetAllFoods()
        {
            var foods = _context.FoodItems.ToList();

            return Ok(new
            {
                message = "Food list fetched",
                data = foods
            });
        }

        [HttpPut("Update/{id}")]
        public IActionResult UpdateFood(int id, FoodDto dto)
        {
            var food = _context.FoodItems.Find(id);

            if (food == null)
                return NotFound("Food not found");

            if (string.IsNullOrWhiteSpace(dto.FoodName))
                return BadRequest("Food name is required");

            food.FoodName = dto.FoodName;

            _context.SaveChanges();

            return Ok(new
            {
                message = "Food updated successfully",
                data = food
            });
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteFood(int id)
        {
            var food = _context.FoodItems.Find(id);

            if (food == null)
                return NotFound("Food not found");

            _context.FoodItems.Remove(food);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Food deleted successfully"
            });
        }
    }
}
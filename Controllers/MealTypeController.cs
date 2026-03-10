using Microsoft.AspNetCore.Mvc;
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
        public IActionResult AddMealType(MealTypeDto dto)
        {
            var mealType = new MealType
            {
                MealName = dto.MealName,
                FixedPrice = dto.FixedPrice
            };

            _context.MealTypes.Add(mealType);
            _context.SaveChanges();

            return Ok(mealType);
        }

        [HttpGet("All")]
        public IActionResult GetAllMealTypes()
        {
            return Ok(_context.MealTypes.ToList());
        }

        [HttpPut("Update/{id}")]
        public IActionResult UpdateMealType(int id, MealType updatedMeal)
        {
            var meal = _context.MealTypes.Find(id);

            if (meal == null)
                return NotFound("Meal type not found");

            meal.MealName = updatedMeal.MealName;
            meal.FixedPrice = updatedMeal.FixedPrice;

            _context.SaveChanges();

            return Ok(meal);
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteMealType(int id)
        {
            var meal = _context.MealTypes.Find(id);

            if (meal == null)
                return NotFound("Meal type not found");

            _context.MealTypes.Remove(meal);
            _context.SaveChanges();

            return Ok("Meal type deleted successfully");
        }
    }
}
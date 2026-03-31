using System.ComponentModel.DataAnnotations;

namespace MealManagement.Models
{
    public class FoodItem
    {
        [Key]
        public int FoodId { get; set; }

        public string FoodName { get; set; } = string.Empty;

        public int MealTypeId { get; set; }

        public MealType? MealType { get; set; }
    }
}
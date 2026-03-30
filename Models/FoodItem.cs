using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealManagement.Models
{
    public class FoodItem
    {
        [Key]
        public int FoodId { get; set; }

        public string FoodName { get; set; }

        public int MealTypeId { get; set; }

        [ForeignKey("MealTypeId")]
        public MealType MealType { get; set; }
    }
}
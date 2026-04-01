using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealManagement.Models
{
    public class MealRecord
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public int MealTypeId { get; set; }

        public int FoodId { get; set; }   

        public DateTime MealDate { get; set; }

        public Employee? Employee { get; set; }
        public MealType? MealType { get; set; }

        [ForeignKey("FoodId")]
        public FoodItem? FoodItem { get; set; }
    }
}
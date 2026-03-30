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

        [ForeignKey("EmployeeId")]
        public Employee ?Employee { get; set; }

        [ForeignKey("MealTypeId")]
        public MealType ?MealType { get; set; }

        [ForeignKey("FoodId")]
        public FoodItem ?Food { get; set; } 
    }
}
using System.ComponentModel.DataAnnotations;

namespace MealManagement.Models
{
    public class MealType
    {
        [Key]
        public int MealTypeId { get; set; }

        public string MealName { get; set; }

        public decimal FixedPrice { get; set; }
    }
}

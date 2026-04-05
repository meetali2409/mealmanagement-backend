using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealManagement.Models
{
    public class MealFood
    {
        [Key]
        public int Id { get; set; }

        public int MealRecordId { get; set; }
        public MealRecord MealRecord { get; set; }

        public int FoodId { get; set; }
        public FoodItem FoodItem { get; set; }
    }
}
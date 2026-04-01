namespace MealManagement.Models
{
    public class AddBulkMealDto
    {
        public int EmployeeId { get; set; }
        public int MealTypeId { get; set; }
        public List<int> FoodIds { get; set; } = new();
    }
}

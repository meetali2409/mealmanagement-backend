public class MealRecord
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public int MealTypeId { get; set; }

    public DateTime MealDate { get; set; }

    public Employee? Employee { get; set; }
    public MealType? MealType { get; set; }

    public List<MealFood> MealFoods { get; set; } = new();
}
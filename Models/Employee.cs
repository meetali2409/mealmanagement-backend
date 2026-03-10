using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace MealManagement.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}

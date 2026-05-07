using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MealManagement.Data;
using MealManagement.Models;

namespace MealManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly MealManagerDbContext _context;

        public EmployeeController(MealManagerDbContext context)
        {
            _context = context;
        }
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API Working");
        }
       
        [HttpPost("Add")]
        public IActionResult AddEmployee(EmployeeDto dto)
        {
            var emp = new Employee
            {
                FullName = dto.FullName
            };

            _context.Employees.Add(emp);
            _context.SaveChanges();

            return Ok(emp);
        }
        [HttpGet("All")]
        public IActionResult GetAllEmployees()
        {
            var employees = _context.Employees.ToList();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployee(int id)
        {
            var employee = _context.Employees.Find(id);

            if (employee == null)
                return NotFound("Employee Not Found");

            return Ok(employee);
        }
        [HttpPut("Update/{id}")]
        public IActionResult UpdateEmployee(int id, UpdateEmployeeDto dto)
        {
            var employee = _context.Employees.Find(id);

            if (employee == null)
                return NotFound("Employee not found");

            employee.FullName = dto.FullName;

            _context.SaveChanges();

            return Ok(employee);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = _context.Employees.Find(id);

            if (employee == null)
                return NotFound("Employee Not Found");

            var meals = _context.MealRecords
                .Where(m => m.EmployeeId == id)
                .ToList();

            _context.MealRecords.RemoveRange(meals);
            _context.Employees.Remove(employee);

            _context.SaveChanges();

            return Ok("Deleted Successfully");
        }
    }
}
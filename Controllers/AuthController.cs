using MealManagement.Data;
using MealManagement.Models;
using MealManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MealManagerDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(
            MealManagerDbContext context,
            JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }
        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = _context.Employees
                    .FirstOrDefault(e => e.Email == model.Email);

                if (existingUser != null)
                {
                    return BadRequest(new
                    {
                        message = "Email already registered"
                    });
                }

                var employee = new Employee
                {
                    FullName = model.FullName,
                    Email = model.Email,

                    Password = BCrypt.Net.BCrypt.HashPassword(
                        model.Password
                    ),

                    Role = "User"
                };

                _context.Employees.Add(employee);
                _context.SaveChanges();

                return Ok(new
                {
                    message = "Registered Successfully",
                    employee.EmployeeId,
                    employee.FullName,
                    employee.Email,
                    employee.Role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Server error",
                    error = ex.Message
                });
            }
        }
        [HttpPost("login")]
        public IActionResult Login(LoginDto model)
        {
            var user = _context.Employees
                .FirstOrDefault(x => x.Email == model.Email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "User not found"
                });
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
                model.Password,
                user.Password
            );

            if (!isPasswordValid)
            {
                return Unauthorized(new
                {
                    message = "Invalid password"
                });
            }

            var token = _jwtService.GenerateToken(
                    user.Email,
                    user.EmployeeId,
                    user.Role
);

            return Ok(new
            {
                token = token,
                message = "Login successful"
            });
        }
    }
}
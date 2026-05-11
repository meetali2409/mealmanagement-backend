using MailKit.Net.Smtp;
using MailKit.Security;
using MealManagement.Data;
using MealManagement.Models;
using MealManagement.Services;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Data;

namespace MealManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MealManagerDbContext _context;
        private readonly JwtService _jwtService;

        private static Dictionary<string, string> otpStore
            = new Dictionary<string, string>();

        public AuthController(
            MealManagerDbContext context,
            JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("SendOtp")]
        public IActionResult SendOtp(
            [FromBody] string email)
        {
            try
            {
                if (!email.EndsWith("@gmail.com"))
                {
                    return BadRequest(new
                    {
                        message = "Only Gmail allowed"
                    });
                }

                Random random = new Random();

                string otp = random
                    .Next(100000, 999999)
                    .ToString();

                otpStore[email] = otp;

                var message = new MimeMessage();

                message.From.Add(
                    MailboxAddress.Parse(
                        "meetali249@gmail.com"
                    )
                );

                message.To.Add(
                    MailboxAddress.Parse(email)
                );

                message.Subject =
                    "Meal Management OTP";

                message.Body = new TextPart("plain")
                {
                    Text = $"Your OTP is: {otp}"
                };

                using var smtp = new SmtpClient();

                smtp.Connect(
                    "smtp.gmail.com",
                    587,
                    SecureSocketOptions.StartTls
                );

                smtp.Authenticate(
                    "meetali249@gmail.com",
                    "klto bxar zoil racy"
                );

                smtp.Send(message);

                smtp.Disconnect(true);

                return Ok(new
                {
                    message =
                        "OTP Sent Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("SendResetOtp")]
        public IActionResult SendResetOtp(
            [FromBody] string email)
        {
            try
            {
                var user = _context.Employees
                    .FirstOrDefault(x => x.Email == email);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        message = "Email not found"
                    });
                }

                Random random = new Random();

                string otp = random
                    .Next(100000, 999999)
                    .ToString();

                otpStore[email] = otp;

                var message = new MimeMessage();

                message.From.Add(
                    MailboxAddress.Parse(
                        "meetali249@gmail.com"
                    )
                );

                message.To.Add(
                    MailboxAddress.Parse(email)
                );

                message.Subject =
                    "Password Reset OTP";

                message.Body = new TextPart("plain")
                {
                    Text =
                        $"Your password reset OTP is: {otp}"
                };

                using var smtp = new SmtpClient();

                smtp.Connect(
                    "smtp.gmail.com",
                    587,
                    SecureSocketOptions.StartTls
                );

                smtp.Authenticate(
                    "meetali249@gmail.com",
                    "klto bxar zoil racy"
                );

                smtp.Send(message);

                smtp.Disconnect(true);

                return Ok(new
                {
                    message =
                        "Reset OTP sent successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword(
            [FromBody] ResetPasswordDto model)
        {
            try
            {
                if (!otpStore.ContainsKey(model.Email))
                {
                    return BadRequest(new
                    {
                        message = "OTP not found"
                    });
                }

                if (
                    otpStore[model.Email]
                    != model.Otp
                )
                {
                    return BadRequest(new
                    {
                        message = "Invalid OTP"
                    });
                }

                var user = _context.Employees
                    .FirstOrDefault(
                        x => x.Email == model.Email
                    );

                if (user == null)
                {
                    return BadRequest(new
                    {
                        message = "User not found"
                    });
                }

                user.Password =
                    BCrypt.Net.BCrypt.HashPassword(
                        model.NewPassword
                    );

                _context.SaveChanges();

                otpStore.Remove(model.Email);

                return Ok(new
                {
                    message =
                        "Password reset successful"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("Register")]
        public IActionResult Register(
            [FromBody] RegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = _context.Employees
                    .FirstOrDefault(
                        e => e.Email == model.Email
                    );

                if (existingUser != null)
                {
                    return BadRequest(new
                    {
                        message =
                            "Email already registered"
                    });
                }

                if (!otpStore.ContainsKey(model.Email))
                {
                    return BadRequest(new
                    {
                        message = "OTP not sent"
                    });
                }

                if (
                    otpStore[model.Email]
                    != model.Otp
                )
                {
                    return BadRequest(new
                    {
                        message = "Invalid OTP"
                    });
                }

                var employee = new Employee
                {
                    FullName = model.FullName,

                    Email = model.Email,

                    Password =
                        BCrypt.Net.BCrypt.HashPassword(
                            model.Password
                        ),

                    Role = "user"
                };

                _context.Employees.Add(employee);

                _context.SaveChanges();

                otpStore.Remove(model.Email);

                return Ok(new
                {
                    message =
                        "Registered Successfully",

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
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
        [HttpPost("login")]
        public IActionResult Login(LoginDto model)
        {try
            {var user = _context.Employees
                    .FirstOrDefault(
                        x => x.Email == model.Email
                    );
                if (user == null)
                {
                    return Unauthorized(new
                    {
                        message = "User not found"
                    });
                }
                bool isPasswordValid =
                    BCrypt.Net.BCrypt.Verify(
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
                var token =
                    _jwtService.GenerateToken(
                        user.Email,
                        user.EmployeeId,
                        user.Role
                    );
                return Ok(new
                {
                    token = token,
                    message =
                        "Login successful",
                    employee = new
                    {
                        user.EmployeeId,
                        user.FullName,
                        user.Email,
                        user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }
    }
}
using API.Models;
using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Repositories;
using API.Models.DataBinding;
using API.Helpers.Enums;

namespace PaperNest_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var new_user_obj = new User
            {
                Name = newUser.Name,
                Email = newUser.Email,
                Password = newUser.Password,
                Role = string.IsNullOrEmpty(newUser.Role) ? UserRole.Student.ToString() : UserRole.Lecturer.ToString()
            };

            UserService.Create(new_user_obj);

            return CreatedAtAction(nameof(Register), new { id = newUser.Id }, new
            {
                message = "User successfully create account",
                data = new_user_obj
            });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest existingUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorizedUser = UserRepository.userRepository.FirstOrDefault(u => (u.Email!.ToLowerInvariant() == existingUser.Email!.ToLowerInvariant()) && (u.Password == existingUser.Password));

            if (authorizedUser == null)
            {
                return Unauthorized(new
                {
                    message = "Email or Password incorrect"
                });
            }

            return Ok(new
            {
                message = "User Successfully Login",
                data = new
                {
                    authorizedUser.Email,
                    authorizedUser.Password
                }
            });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(string userEmail, string newPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool success = UserService.ResetPassword(userEmail, newPassword);

            if (!success)
            {
                return NotFound(new
                {
                    message = "User not found [Reset Password Gagal]"
                });
            }

            return Ok(new
            {
                message = "Password successfully resetted"
            });
        }

        [HttpPost("change-email")]
        public IActionResult ResetEmail([FromBody] ChangeEmailRequest newEmail)
        {
            if (string.IsNullOrEmpty(newEmail.OldEmail) || string.IsNullOrEmpty(newEmail.NewEmail))
            {
                return BadRequest(ModelState);
            }

            UserService.ChangeEmail(newEmail.OldEmail, newEmail.NewEmail);

            return Ok(new
            {
                message = "Email successfully changed"
            });
        }
    }
}

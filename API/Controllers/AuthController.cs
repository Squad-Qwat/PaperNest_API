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
    public class AuthController(UserService userService) : Controller
    {
        private readonly UserService _userService = userService;

        /*
         * Setara dengan:
         * public AuthController(UserService userService)
         * {
         *    _userService = userService;
         * }
         */

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest newUser)
        {
            if (!ModelState.IsValid)
            {
                return Unauthorized(ModelState);
            }

            if (newUser == null)
            {
                return BadRequest(new
                {
                    message = "Invalid user data"
                });
            }

            var new_user_obj = _userService.Register(newUser.Email, newUser.Password, newUser.Name, newUser.Username, newUser.Role);

            if (new_user_obj == null)
            {
                return BadRequest(new
                {
                    message = "User already exists with the same email or username"
                });
            }

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
                return Unauthorized(ModelState);
            }

            if (existingUser == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid user data"
                });
            }

            // Check if email and password are provided
            if (string.IsNullOrEmpty(existingUser.Email) || string.IsNullOrEmpty(existingUser.Password))
            {
                return BadRequest(new
                {
                    message = "Email and Password must be provided"
                });
            }

            var authorizedUser = _userService.Login(existingUser.Email, existingUser.Password);

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
                return Unauthorized(ModelState);
            }

            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest(new
                {
                    message = "Email and new password must be provided"
                });
            }

            bool success = _userService.ResetPassword(userEmail, newPassword);

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
            if (!ModelState.IsValid)
            {
                return Unauthorized(ModelState);
            }
            
            if (newEmail == null)
            {
                return BadRequest(new
                {
                    message = "Invalid email data"
                });
            }

            if (string.IsNullOrEmpty(newEmail.OldEmail) || string.IsNullOrEmpty(newEmail.NewEmail))
            {
                return Unauthorized(ModelState);
            }

            _userService.ChangeEmail(newEmail.OldEmail, newEmail.NewEmail);

            return Ok(new
            {
                message = "Email successfully changed"
            });
        }
    }
}
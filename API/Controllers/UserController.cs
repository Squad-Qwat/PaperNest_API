using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;

namespace PaperNest_API.Controllers
{
    [ApiController, Route("/api/users")]
    public class UserController : Controller
    {
        public readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetAllUser()
        {
            var user = _userService.GetAll();

            if (user == null || !user.Any())
            {
                return NotFound(new
                {
                    message = "No users found"
                });
            }

            return Ok(new
            {
                message = "Success get all User data",
                data = user
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "User ID cannot be empty"
                });
            }

            var user = _userService.GetById(id);

            if (user == null)
            {
                return NotFound(new 
                {
                    message = "User not found!"
                });
            }

            return Ok(new
            {
                message = "Success get all User data with id: " + user.Id,
                data = user
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(Guid id, [FromBody] User user)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "User ID cannot be empty"
                });
            }

            if (user == null)
            {
                return BadRequest(new
                {
                    message = "User data cannot be null"
                });
            }

            var existingUser = _userService.GetById(id);

            if (existingUser == null)
            {
                return NotFound(new 
                { 
                    message = "User doesn't exist!"
                });
            }

            if (existingUser.Name == null || existingUser.Username == null)
            {
                return BadRequest(new
                {
                    message = "User name and username cannot be null"
                });
            }

            existingUser.Name = user.Name;
            existingUser.Username = user.Username;

            return Ok(new
            {
                message = "Success to update user with id: " + existingUser.Id
            });
        }

        [HttpDelete("id")]
        public IActionResult DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "User ID cannot be empty"
                });
            }

            _userService.Delete(id);

            return Ok(new
            {
                message = "User has been deleted"
            });
        }
    }
}
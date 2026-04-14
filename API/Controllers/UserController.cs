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

            return Ok(new
            {
                message = "Success get all User data",
                data = user
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(Guid id)
        {
            var user = _userService.GetById(id);

            if (user == null)
            {
                return NotFound();
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
            var existingUser = _userService.GetById(id);

            if (existingUser == null)
            {
                return NotFound();
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
            _userService.Delete(id);

            return Ok(new
            {
                message = "User has been deleted"
            });
        }
    }
}

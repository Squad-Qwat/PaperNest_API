using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;

namespace PaperNest_API.Controllers
{
    [ApiController, Route("/api/users")]
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult GetAllUser()
        {
            var user = UserService.GetAll();

            return Ok(new
            {
                message = "Success get all User data",
                data = user
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(Guid id)
        {
            var user = UserService.GetById(id);

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
            var existingUser = UserService.GetById(id);

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
            UserService.Delete(id);

            return Ok(new
            {
                message = "User has been deleted"
            });
        }
    }
}

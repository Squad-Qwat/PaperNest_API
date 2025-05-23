using Microsoft.AspNetCore.Mvc;
using API.Services;

namespace API.Controllers
{
    [ApiController,
        Route("api/user/workspace")
        ]
    public class UserWorkspaceController : Controller
    {
        public readonly UserWorkspaceService _userWorkspaceService;
        public UserWorkspaceController(UserWorkspaceService userWorkspaceService)
        {
            _userWorkspaceService = userWorkspaceService;
        }
        
        [HttpGet]
        public IActionResult GetAllUserWorkspaces()
        {
            var userWorkspaces = _userWorkspaceService.GetAllUserWorkspaces();
            return Ok(userWorkspaces);
        }
        
        [HttpGet("id/{id}")]
        public IActionResult GetUserWorkspaceById(Guid id)
        {
            try
            {
                var userWorkspace = _userWorkspaceService.GetUserWorkspaceById(id);
                if (userWorkspace == null)
                {
                    return NotFound(new { message = "Relasi user-workspace tidak ditemukan" });
                }
                return Ok(userWorkspace);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil relasi user-workspace" });
            }
        }

        [HttpGet("{userId}")]
        public IActionResult GetUserWorkspaces(Guid userId)
        {
            try
            {
                var workspaces = _userWorkspaceService.GetUserWorkspacesByUserId(userId);
                if (workspaces == null || !workspaces.Any())
                {
                    return NotFound(new { message = "User tidak memiliki workspace" });
                }
                return Ok(workspaces);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil workspace user" });
            }
        }

        [HttpPost("{userId}/{workspaceId}")]
        public IActionResult AddUserWorkspaceAsOwner(Guid userId, Guid workspaceId)
        {
            _userWorkspaceService.AddUserWorkspaceAsOwner(userId, workspaceId);
            return CreatedAtAction(nameof(GetUserWorkspaces), new { userId }, null);
        }

        [HttpPost("{userId}/{workspaceId}/join")]
        public IActionResult AddUserWorkspaceAsMember(Guid userId, Guid workspaceId)
        {
            _userWorkspaceService.AddUserWorkspaceAsMember(userId, workspaceId);
            return CreatedAtAction(nameof(GetUserWorkspaces), new { userId }, null);
        }

        [HttpPost("{userId}/{workspaceId}/lecturer/join")]
        public IActionResult AddUserWorkspaceAsLecturer(Guid userId, Guid workspaceId)
        {
            _userWorkspaceService.AddUserWorkspaceAsLecturer(userId, workspaceId);
            return CreatedAtAction(nameof(GetUserWorkspaces), new { userId }, null);
        }

        [HttpDelete("{userId}/{workspaceId}")]
        public IActionResult RemoveUserWorkspace(Guid userId, Guid workspaceId)
        {
            var isRemoved = _userWorkspaceService.RemoveUserWorkspace(userId, workspaceId);
            if (isRemoved)
            {
                return Ok(new
                {
                    message = "Berhasil menghapus user dari workspace"
                });
            }
            else
            {
                return BadRequest(new
                {
                    message = "Gagal menghapus user dari workspace"
                });
            }
        }
    }
}

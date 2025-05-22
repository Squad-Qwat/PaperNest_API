using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Helpers.Enums;

namespace API.Controllers
{
    [ApiController,
        Route("api/user/workspace")
        ]
    public class UserWorkspaceController(UserWorkspaceService userWorkspaceService) : Controller
    {
        public readonly UserWorkspaceService _userWorkspaceService = userWorkspaceService;

        /*
         * Setara dengan:
         * public UserWorkspaceController(UserWorkspaceService userWorkspaceService)
         * {
         *    _userWorkspaceService = userWorkspaceService;
         * }
         */

        [HttpGet("{userId}")]
        public IActionResult GetUserWorkspaces(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "UserId tidak boleh kosong" });
            }

            var workspaces = _userWorkspaceService.GetUserWorkspacesByUserId(userId);
            
            if (workspaces == null || !workspaces.Any())
            {
                return NotFound(new 
                { 
                    message = $"Tidak ada workspace yang ditemukan untuk user dengan id {userId}"
                });
            }
            
            return Ok(workspaces);
        }

        [HttpPost("{userId}/{workspaceId}")]
        public IActionResult AddUserWorkspaceAsOwner(Guid userId, Guid workspaceId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "UserId tidak boleh kosong" });
            }

            if (workspaceId == Guid.Empty)
            {
                return BadRequest(new { message = "WorkspaceId tidak boleh kosong" });
            }

            _userWorkspaceService.AddUserWorkspaceAsOwner(userId, workspaceId);
            return CreatedAtAction(nameof(GetUserWorkspaces), new { userId }, null);
        }

        [HttpPost("{userId}/{workspaceId}/join")]
        public IActionResult AddUserWorkspaceAsMember(Guid userId, Guid workspaceId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "UserId tidak boleh kosong" });
            }

            if (workspaceId == Guid.Empty)
            {
                return BadRequest(new { message = "WorkspaceId tidak boleh kosong" });
            }

            _userWorkspaceService.AddUserWorkspaceAsMember(userId, workspaceId);
            return CreatedAtAction(nameof(GetUserWorkspaces), new { userId }, null);
        }

        [HttpPost("{userId}/{workspaceId}/lecturer/join")]
        public IActionResult AddUserWorkspaceAsLecturer(Guid userId, Guid workspaceId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "UserId tidak boleh kosong" });
            }

            if (workspaceId == Guid.Empty)
            {
                return BadRequest(new { message = "WorkspaceId tidak boleh kosong" });
            }

            _userWorkspaceService.AddUserWorkspaceAsLecturer(userId, workspaceId);
            return CreatedAtAction(nameof(GetUserWorkspaces), new { userId }, null);
        }

        [HttpPut("{userId}/{workspaceId}")]
        public IActionResult UpdateUserWorkspace(Guid userId, Guid workspaceId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "UserId tidak boleh kosong" });
            }
            if (workspaceId == Guid.Empty)
            {
                return BadRequest(new { message = "WorkspaceId tidak boleh kosong" });
            }
            
            var isUpdated = _userWorkspaceService.UpdateUserWorkspace(userId, workspaceId);
            if (!isUpdated)
            {
                return BadRequest(new
                {
                    message = "Gagal memperbarui user di workspace"
                });
            }
            return Ok(new
            {
                message = "Berhasil memperbarui user di workspace"
            });
        }

        [HttpPut("{userId}/{workspaceId}/role/{role}")]
        public IActionResult UpdateUserWorkspaceRole(Guid userId, Guid workspaceId, string role)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "UserId tidak boleh kosong" });
            }

            if (workspaceId == Guid.Empty)
            {
                return BadRequest(new { message = "WorkspaceId tidak boleh kosong" });
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                return BadRequest(new { message = "Role tidak boleh kosong" });
            }

            if (!Enum.TryParse(role, true, out WorkspaceRole workspaceRole))
            {
                return BadRequest(new { message = "Role tidak valid" });
            }

            var isUpdated = _userWorkspaceService.UpdateUserWorkspaceRole(userId, workspaceId, workspaceRole);
            if (!isUpdated)
            {
                return BadRequest(new
                {
                    message = "Gagal memperbarui role user di workspace"
                });
            }

            return Ok(new
            {
                message = "Berhasil memperbarui role user di workspace"
            });
        }

        [HttpDelete("{userId}/{workspaceId}")]
        public IActionResult RemoveUserWorkspace(Guid userId, Guid workspaceId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "UserId tidak boleh kosong" });
            }

            if (workspaceId == Guid.Empty)
            {
                return BadRequest(new { message = "WorkspaceId tidak boleh kosong" });
            }

            var isRemoved = _userWorkspaceService.RemoveUserWorkspace(userId, workspaceId);
            if (!isRemoved)
            {
                return BadRequest(new
                {
                    message = "Gagal menghapus user dari workspace"
                });
            }
            
            return Ok(new
            {
                message = "Berhasil menghapus user dari workspace"
            });
        }
    }
}
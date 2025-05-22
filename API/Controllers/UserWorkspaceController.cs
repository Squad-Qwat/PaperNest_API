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

        [HttpGet("{userId}")]
        public IActionResult GetUserWorkspaces(Guid userId)
        {
            var workspaces = _userWorkspaceService.GetUserWorkspacesByUserId(userId);
            if (workspaces == null || !workspaces.Any())
            {
                return NotFound();
            }
            return Ok(workspaces);
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
    }
}

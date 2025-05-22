using Microsoft.AspNetCore.Mvc;
using API.Helpers.Enums;
using API.Models;
using API.Services;

namespace PaperNest_API.Controllers
{
    [ApiController, Route("api/workspaces")]
    public class WorkspaceController : Controller
    {
        [HttpGet]
        public IActionResult GetAllWorkspaces()
        {
            var workspaces = WorkspaceService.GetAll();

            return Ok(new
            {
                message = "Success get all workspace",
                data = workspaces
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetWorkspaceById(Guid workspaceId)
        {
            var workspace = WorkspaceService.GetById(workspaceId);

            if (workspace == null)
            {
                return NotFound(new
                {
                    message = "Workspace not found"
                });
            }

            return Ok(new
            {
                message = "Success get workspace data",
                data = workspace
            });
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetWorkspacesByUserId(Guid userId)
        {
            var workspaces = WorkspaceService.GetByUserId(userId);

            return Ok(new
            {
                message = "Success get user's workspace",
                data = workspaces
            });
        }

        [HttpGet("joined/{userId}")]
        public IActionResult GetJoinedWorkspaces(Guid userId)
        {
            var workspaces = WorkspaceService.GetJoinedWorkspaces(userId);

            return Ok(new
            {
                message = "Success get joined workspace",
                data = workspaces
            });
        }

        [HttpPost]
        public IActionResult CreateWorkspace([FromBody] Workspace workspace)
        {
            WorkspaceService.Create(workspace);

            return CreatedAtAction(nameof(GetWorkspaceById), new { id = workspace.Id }, new
            {
                message = "Success create new workspace",
                data = workspace
            });
        }

        [HttpPost("join")]
        public IActionResult JoinWorkspace(Guid workspaceId, Guid userId, WorkspaceRole role = WorkspaceRole.Lecturer)
        {
            var userWorkspace = WorkspaceService.JoinWorkspace(workspaceId, userId, role);
            
            if (userWorkspace == null)
            {
                return NotFound(new
                {
                    message = "Workspace not found"
                });
            }
            
            var workspace = WorkspaceService.GetById(workspaceId);
            
            return Ok(new
            {
                message = "Success joined workspace",
                data = new
                {
                    workspace = workspace,
                    userWorkspace = userWorkspace
                }
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateWorkspace(Guid id, [FromBody] Workspace workspace)
        {
            var existingWorkspace = WorkspaceService.GetById(id);

            if (existingWorkspace == null)
            {
                return NotFound(new
                {
                    message = "Workspace not found"
                });
            }

            WorkspaceService.Update(id, workspace);

            return Ok(new
            {
                message = "Success update workspace",
                data = WorkspaceService.GetById(id)
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteWorkspace(Guid id)
        {
            var existingWorkspace = WorkspaceService.GetById(id);

            if (existingWorkspace == null)
            {
                return NotFound(new
                {
                    message = "Workspace not found"
                });
            }

            WorkspaceService.Delete(id);

            return Ok(new
            {
                message = "Workspace has been deleted"
            });
        }
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Api.Common;
using ProjectManagement.Api.Domain;
using ProjectManagement.Api.Services;

namespace ProjectManagement.Api.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController(ProjectService projectService) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> List() =>
        (await projectService.GetUserProjectsAsync(CurrentUserId)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest req) =>
        (await projectService.CreateProjectAsync(CurrentUserId, req.Name, req.Description)).ToActionResult(created: true);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id) =>
        (await projectService.GetProjectAsync(id, CurrentUserId)).ToActionResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest req) =>
        (await projectService.UpdateProjectAsync(id, CurrentUserId, req.Name, req.Description)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) =>
        (await projectService.DeleteProjectAsync(id, CurrentUserId)).ToActionResult();

    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddMemberRequest req) =>
        (await projectService.AddMemberAsync(id, CurrentUserId, req.UserId, req.Role)).ToActionResult();

    [HttpDelete("{id:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId) =>
        (await projectService.RemoveMemberAsync(id, CurrentUserId, userId)).ToActionResult();
}

public record CreateProjectRequest(string Name, string? Description);
public record UpdateProjectRequest(string Name, string? Description);
public record AddMemberRequest(Guid UserId, Role Role);

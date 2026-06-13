using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Api.Common;
using ProjectManagement.Api.Domain;
using ProjectManagement.Api.Services;

namespace ProjectManagement.Api.Controllers;

[ApiController]
[Authorize]
public class TasksController(TaskService taskService) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("api/projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> List(
        Guid projectId,
        [FromQuery] Domain.TaskStatus? status,
        [FromQuery] Guid? assigneeId) =>
        (await taskService.GetTasksAsync(projectId, CurrentUserId, status, assigneeId)).ToActionResult();

    [HttpPost("api/projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> Create(Guid projectId, [FromBody] CreateTaskRequest req) =>
        (await taskService.CreateTaskAsync(projectId, CurrentUserId, new(req.Title, req.Description, req.Priority, req.AssigneeId, req.DueDate))).ToActionResult(created: true);

    [HttpGet("api/tasks/{id:guid}")]
    public async Task<IActionResult> Get(Guid id) =>
        (await taskService.GetTaskAsync(id, CurrentUserId)).ToActionResult();

    [HttpPut("api/tasks/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest req) =>
        (await taskService.UpdateTaskAsync(id, CurrentUserId, new(req.Title, req.Description, req.Status, req.Priority, req.AssigneeId, req.DueDate))).ToActionResult();

    [HttpDelete("api/tasks/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) =>
        (await taskService.DeleteTaskAsync(id, CurrentUserId)).ToActionResult();
}

public record CreateTaskRequest(string Title, string? Description, Priority Priority, Guid? AssigneeId, DateTime? DueDate);
public record UpdateTaskRequest(string? Title, string? Description, Domain.TaskStatus? Status, Priority? Priority, Guid? AssigneeId, DateTime? DueDate);

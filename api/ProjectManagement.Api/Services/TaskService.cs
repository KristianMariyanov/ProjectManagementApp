using Microsoft.EntityFrameworkCore;
using ProjectManagement.Api.Common;
using ProjectManagement.Api.Data;
using ProjectManagement.Api.Domain;

namespace ProjectManagement.Api.Services;

public class TaskService(AppDbContext db)
{
    private async Task<bool> IsMemberAsync(Guid projectId, Guid userId) =>
        await db.ProjectMembers.AnyAsync(m => m.ProjectId == projectId && m.UserId == userId);

    public async Task<Result<List<TaskItem>>> GetTasksAsync(Guid projectId, Guid userId, Domain.TaskStatus? status, Guid? assigneeId)
    {
        if (!await IsMemberAsync(projectId, userId))
            return Result<List<TaskItem>>.Forbidden("Not a member of this project");

        var query = db.TaskItems.Where(t => t.ProjectId == projectId);
        if (status.HasValue) query = query.Where(t => t.Status == status.Value);
        if (assigneeId.HasValue) query = query.Where(t => t.AssigneeId == assigneeId.Value);

        var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        return Result<List<TaskItem>>.Ok(tasks);
    }

    public async Task<Result<TaskItem>> GetTaskAsync(Guid taskId, Guid userId)
    {
        var task = await db.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task is null) return Result<TaskItem>.NotFound("Task not found");
        if (!await IsMemberAsync(task.ProjectId, userId))
            return Result<TaskItem>.Forbidden("Not a member of this project");
        return Result<TaskItem>.Ok(task);
    }

    public async Task<Result<TaskItem>> CreateTaskAsync(Guid projectId, Guid userId, CreateTaskDto dto)
    {
        if (!await IsMemberAsync(projectId, userId))
            return Result<TaskItem>.Forbidden("Not a member of this project");

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = dto.Title,
            Description = dto.Description,
            Status = Domain.TaskStatus.Todo,
            Priority = dto.Priority,
            AssigneeId = dto.AssigneeId,
            DueDate = dto.DueDate,
            CreatedAt = DateTime.UtcNow,
        };

        db.TaskItems.Add(task);
        await db.SaveChangesAsync();
        return Result<TaskItem>.Ok(task);
    }

    public async Task<Result<TaskItem>> UpdateTaskAsync(Guid taskId, Guid userId, UpdateTaskDto dto)
    {
        var task = await db.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task is null) return Result<TaskItem>.NotFound("Task not found");
        if (!await IsMemberAsync(task.ProjectId, userId))
            return Result<TaskItem>.Forbidden("Not a member of this project");

        task.Title = dto.Title ?? task.Title;
        task.Description = dto.Description ?? task.Description;
        if (dto.Status.HasValue) task.Status = dto.Status.Value;
        if (dto.Priority.HasValue) task.Priority = dto.Priority.Value;
        task.AssigneeId = dto.AssigneeId ?? task.AssigneeId;
        task.DueDate = dto.DueDate ?? task.DueDate;

        await db.SaveChangesAsync();
        return Result<TaskItem>.Ok(task);
    }

    public async Task<Result> DeleteTaskAsync(Guid taskId, Guid userId)
    {
        var task = await db.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task is null) return Result.NotFound("Task not found");
        if (!await IsMemberAsync(task.ProjectId, userId))
            return Result.Forbidden("Not a member of this project");

        db.TaskItems.Remove(task);
        await db.SaveChangesAsync();
        return Result.Ok();
    }
}

public record CreateTaskDto(string Title, string? Description, Priority Priority, Guid? AssigneeId, DateTime? DueDate);
public record UpdateTaskDto(string? Title, string? Description, Domain.TaskStatus? Status, Priority? Priority, Guid? AssigneeId, DateTime? DueDate);

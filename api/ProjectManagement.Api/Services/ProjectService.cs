using Microsoft.EntityFrameworkCore;
using ProjectManagement.Api.Common;
using ProjectManagement.Api.Data;
using ProjectManagement.Api.Domain;

namespace ProjectManagement.Api.Services;

public class ProjectService(AppDbContext db)
{
    public async Task<Result<List<ProjectDto>>> GetUserProjectsAsync(Guid userId)
    {
        var projects = await db.Projects
            .Where(p => p.Members.Any(m => m.UserId == userId))
            .Include(p => p.Members)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return Result<List<ProjectDto>>.Ok(projects.Select(ToDto).ToList());
    }

    public async Task<Result<ProjectDto>> GetProjectAsync(Guid projectId, Guid userId)
    {
        var project = await db.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project is null)
            return Result<ProjectDto>.NotFound("Project not found");
        if (!project.Members.Any(m => m.UserId == userId))
            return Result<ProjectDto>.Forbidden("Not a member of this project");

        return Result<ProjectDto>.Ok(ToDto(project));
    }

    public async Task<Result<ProjectDto>> CreateProjectAsync(Guid userId, string name, string? description)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow,
        };
        project.Members.Add(new ProjectMember { ProjectId = project.Id, UserId = userId, Role = Role.Owner });

        db.Projects.Add(project);
        await db.SaveChangesAsync();
        return Result<ProjectDto>.Ok(ToDto(project));
    }

    public async Task<Result<ProjectDto>> UpdateProjectAsync(Guid projectId, Guid userId, string name, string? description)
    {
        var project = await db.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
        if (project is null) return Result<ProjectDto>.NotFound("Project not found");

        var member = project.Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null) return Result<ProjectDto>.Forbidden("Not a member of this project");
        if (member.Role == Role.Viewer) return Result<ProjectDto>.Forbidden("Viewers cannot edit projects");

        project.Name = name;
        project.Description = description;
        await db.SaveChangesAsync();
        return Result<ProjectDto>.Ok(ToDto(project));
    }

    public async Task<Result> DeleteProjectAsync(Guid projectId, Guid userId)
    {
        var project = await db.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
        if (project is null) return Result.NotFound("Project not found");

        var member = project.Members.FirstOrDefault(m => m.UserId == userId);
        if (member?.Role != Role.Owner) return Result.Forbidden("Only the project owner can delete it");

        db.Projects.Remove(project);
        await db.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> AddMemberAsync(Guid projectId, Guid requestingUserId, Guid targetUserId, Role role)
    {
        var project = await db.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
        if (project is null) return Result.NotFound("Project not found");

        var requesting = project.Members.FirstOrDefault(m => m.UserId == requestingUserId);
        if (requesting?.Role != Role.Owner) return Result.Forbidden("Only owners can add members");

        if (project.Members.Any(m => m.UserId == targetUserId))
            return Result.BadRequest("User is already a member");

        var targetExists = await db.Users.AnyAsync(u => u.Id == targetUserId);
        if (!targetExists) return Result.NotFound("User not found");

        project.Members.Add(new ProjectMember { ProjectId = projectId, UserId = targetUserId, Role = role });
        await db.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> RemoveMemberAsync(Guid projectId, Guid requestingUserId, Guid targetUserId)
    {
        var project = await db.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
        if (project is null) return Result.NotFound("Project not found");

        var requesting = project.Members.FirstOrDefault(m => m.UserId == requestingUserId);
        if (requesting?.Role != Role.Owner) return Result.Forbidden("Only owners can remove members");

        var target = project.Members.FirstOrDefault(m => m.UserId == targetUserId);
        if (target is null) return Result.NotFound("Member not found");
        if (target.Role == Role.Owner && project.Members.Count(m => m.Role == Role.Owner) == 1)
            return Result.BadRequest("Cannot remove the last owner");

        project.Members.Remove(target);
        await db.SaveChangesAsync();
        return Result.Ok();
    }

    private static ProjectDto ToDto(Project project) =>
        new(
            project.Id,
            project.Name,
            project.Description,
            project.CreatedById,
            project.CreatedAt,
            project.Members
                .Select(m => new ProjectMemberDto(m.ProjectId, m.UserId, m.Role))
                .ToList());
}

public record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    Guid CreatedById,
    DateTime CreatedAt,
    List<ProjectMemberDto> Members);

public record ProjectMemberDto(Guid ProjectId, Guid UserId, Role Role);

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Api.Domain;

namespace ProjectManagement.Api.Data;

public static class Seed
{
    public static async Task SeedAsync(AppDbContext db, UserManager<User> userManager)
    {
        if (await db.Users.AnyAsync()) return; // Idempotent

        var alice = new User { Id = Guid.NewGuid(), Email = "alice@example.com", UserName = "alice@example.com", DisplayName = "Alice", EmailConfirmed = true, CreatedAt = DateTime.UtcNow };
        await userManager.CreateAsync(alice, "Password1!");

        var bob = new User { Id = Guid.NewGuid(), Email = "bob@example.com", UserName = "bob@example.com", DisplayName = "Bob", EmailConfirmed = true, CreatedAt = DateTime.UtcNow };
        await userManager.CreateAsync(bob, "Password1!");

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Product Launch",
            Description = "Track launch planning work across the team",
            CreatedById = alice.Id,
            CreatedAt = DateTime.UtcNow,
        };
        project.Members.Add(new ProjectMember { ProjectId = project.Id, UserId = alice.Id, Role = Role.Owner });
        project.Members.Add(new ProjectMember { ProjectId = project.Id, UserId = bob.Id, Role = Role.Member });

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var tasks = new[]
        {
            new TaskItem { Id = Guid.NewGuid(), ProjectId = project.Id, Title = "Set up the repo", Status = Domain.TaskStatus.Done, Priority = Priority.High, AssigneeId = alice.Id, CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new TaskItem { Id = Guid.NewGuid(), ProjectId = project.Id, Title = "Build the task board UI", Status = Domain.TaskStatus.InProgress, Priority = Priority.High, AssigneeId = alice.Id, CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new TaskItem { Id = Guid.NewGuid(), ProjectId = project.Id, Title = "Write integration tests for auth", Status = Domain.TaskStatus.Todo, Priority = Priority.Medium, AssigneeId = bob.Id, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new TaskItem { Id = Guid.NewGuid(), ProjectId = project.Id, Title = "Prepare release checklist", Status = Domain.TaskStatus.Todo, Priority = Priority.Medium, CreatedAt = DateTime.UtcNow.AddDays(-1) },
        };

        db.TaskItems.AddRange(tasks);
        await db.SaveChangesAsync();
    }
}

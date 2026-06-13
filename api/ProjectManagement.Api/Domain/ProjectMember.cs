namespace ProjectManagement.Api.Domain;

public class ProjectMember
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public Role Role { get; set; }

    public Project Project { get; set; } = null!;
    public User User { get; set; } = null!;
}

public enum Role
{
    Owner,
    Member,
    Viewer,
}

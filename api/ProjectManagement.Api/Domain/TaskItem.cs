namespace ProjectManagement.Api.Domain;

public class TaskItem
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public Priority Priority { get; set; }
    public Guid? AssigneeId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public User? Assignee { get; set; }
}

public enum TaskStatus
{
    Todo,
    InProgress,
    Done,
}

public enum Priority
{
    Low,
    Medium,
    High,
}

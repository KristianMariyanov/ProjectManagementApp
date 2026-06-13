using Microsoft.AspNetCore.Identity;

namespace ProjectManagement.Api.Domain;

public class User : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<ProjectMember> Memberships { get; set; } = new List<ProjectMember>();
}

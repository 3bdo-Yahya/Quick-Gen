using Microsoft.AspNetCore.Identity;

namespace Quick_Gen.Models;

public sealed class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}

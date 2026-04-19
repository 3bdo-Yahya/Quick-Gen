namespace Quick_Gen.Models;

public sealed class Certificate
{
    public int Id { get; set; }

    public string CertificateNumber { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public int? CourseId { get; set; }

    public Course? Course { get; set; }

    public int? LearningPathId { get; set; }

    public LearningPath? LearningPath { get; set; }

    public DateTimeOffset IssuedAt { get; set; }

    public string? RecipientDisplayName { get; set; }

    public string? CourseTitle { get; set; }
}

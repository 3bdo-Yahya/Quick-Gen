namespace Quick_Gen.Models;

public sealed class LearningPath
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? IconUrl { get; set; }

    public CourseDifficulty? PathBadge { get; set; }

    public int DurationWeeks { get; set; }

    public bool IsFree { get; set; } = true;

    public ICollection<PathCourse> PathCourses { get; set; } = new List<PathCourse>();

    public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}

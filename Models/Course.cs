namespace Quick_Gen.Models;

public sealed class Course
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public string? ThumbnailUrl { get; set; }

    public CourseDifficulty Difficulty { get; set; }

    public int CatalogLevel { get; set; } = 1;

    public int DurationWeeks { get; set; }

    public int LessonCount { get; set; }

    public bool IsLocked { get; set; }

    public CourseStatus Status { get; set; } = CourseStatus.Draft;

    public bool IsFree { get; set; } = true;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public ICollection<PathCourse> PathLinks { get; set; } = new List<PathCourse>();

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}

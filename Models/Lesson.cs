namespace Quick_Gen.Models;

public sealed class Lesson
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public int? DurationMinutes { get; set; }

    public string? ContentUrl { get; set; }
}

namespace Quick_Gen.Models;

public sealed class PathCourse
{
    public int Id { get; set; }

    public int LearningPathId { get; set; }

    public LearningPath LearningPath { get; set; } = null!;

    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public int SortOrder { get; set; }
}

using Quick_Gen.Models;

namespace Quick_Gen.Contracts.Courses
{

    public sealed record CreateCourseRequest(
        string Title,
        string ShortDescription,
        string? ThumbnailUrl,
        CourseDifficulty Difficulty,
        int CatalogLevel,
        int DurationWeeks,
        bool IsLocked,
        bool IsFree
    );
}

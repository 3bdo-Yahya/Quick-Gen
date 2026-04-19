namespace Quick_Gen.Contracts.Courses
{
    public sealed record CourseResponse(
        int Id,
        string Title,
        string ShortDescription,
        string? ThumbnailUrl,
        int CatalogLevel,
        int DurationWeeks,
        int LessonCount,
        bool IsLocked,
        bool IsFree
    );
}

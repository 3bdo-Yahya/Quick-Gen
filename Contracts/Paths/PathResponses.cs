using Quick_Gen.Models;

namespace Quick_Gen.Contracts.Paths;

public sealed record PathListItemResponse(
    int Id,
    string Title,
    string Description,
    string? IconUrl,
    string? PathBadge,
    int DurationWeeks,
    bool IsFree,
    int CourseCount
);

public sealed record PathDetailResponse(
    int Id,
    string Title,
    string Description,
    string? IconUrl,
    string? PathBadge,
    int DurationWeeks,
    bool IsFree,
    IReadOnlyList<PathCourseItemResponse> Courses
);

public sealed record PathCourseItemResponse(
    int CourseId,
    int SortOrder,
    string Title,
    string ShortDescription,
    string? ThumbnailUrl,
    string Difficulty,
    int CatalogLevel,
    int DurationWeeks,
    int LessonCount,
    bool IsLocked,
    bool IsFree
);


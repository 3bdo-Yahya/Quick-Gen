using System.ComponentModel.DataAnnotations;
using Quick_Gen.Models;

namespace Quick_Gen.Contracts.Paths;

public sealed record CreatePathRequest(
    [property: Required, MaxLength(500)] string Title,
    [property: Required, MaxLength(8000)] string Description,
    string? IconUrl,
    CourseDifficulty? PathBadge,
    [property: Range(0, int.MaxValue)] int DurationWeeks,
    bool IsFree
);

public sealed record UpdatePathRequest(
    [property: Required, MaxLength(500)] string Title,
    [property: Required, MaxLength(8000)] string Description,
    string? IconUrl,
    CourseDifficulty? PathBadge,
    [property: Range(0, int.MaxValue)] int DurationWeeks,
    bool IsFree
);

public sealed record PathCourseOrderItem(
    [property: Range(1, int.MaxValue)] int CourseId,
    [property: Range(1, int.MaxValue)] int SortOrder
);

public sealed record ReplacePathCoursesRequest(
    [property: Required] IReadOnlyList<PathCourseOrderItem> Courses
);


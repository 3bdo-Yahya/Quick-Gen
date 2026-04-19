using Microsoft.EntityFrameworkCore;
using Quick_Gen.Contracts.Paths;
using Quick_Gen.Data;
using Quick_Gen.Models;
using Quick_Gen.Services.IServices;

namespace Quick_Gen.Services;

public sealed class PathService(ApplicationDbContext db) : IPathService
{
    public async Task<IReadOnlyList<PathListItemResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await db.LearningPaths
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => new PathListItemResponse(
                p.Id,
                p.Title,
                p.Description,
                p.IconUrl,
                p.PathBadge.HasValue ? p.PathBadge.Value.ToString() : null,
                p.DurationWeeks,
                p.IsFree,
                p.PathCourses.Count(pc => pc.Course.Status == CourseStatus.Published)))
            .ToListAsync(cancellationToken);
    }

    public async Task<PathDetailResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await db.LearningPaths
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PathDetailResponse(
                p.Id,
                p.Title,
                p.Description,
                p.IconUrl,
                p.PathBadge.HasValue ? p.PathBadge.Value.ToString() : null,
                p.DurationWeeks,
                p.IsFree,
                p.PathCourses
                    .Where(pc => pc.Course.Status == CourseStatus.Published)
                    .OrderBy(pc => pc.SortOrder)
                    .ThenBy(pc => pc.CourseId)
                    .Select(pc => new PathCourseItemResponse(
                        pc.CourseId,
                        pc.SortOrder,
                        pc.Course.Title,
                        pc.Course.ShortDescription,
                        pc.Course.ThumbnailUrl,
                        pc.Course.Difficulty.ToString(),
                        pc.Course.CatalogLevel,
                        pc.Course.DurationWeeks,
                        pc.Course.LessonCount,
                        pc.Course.IsLocked,
                        pc.Course.IsFree))
                    .ToList()))
            .SingleOrDefaultAsync(cancellationToken);
    }
}


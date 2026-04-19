using Microsoft.EntityFrameworkCore;
using Quick_Gen.Contracts.Paths;
using Quick_Gen.Data;
using Quick_Gen.Models;
using Quick_Gen.Services.IServices;

namespace Quick_Gen.Services;

public sealed class AdminPathService(ApplicationDbContext db) : IAdminPathService
{
    public async Task<PathDetailResponse> CreateAsync(CreatePathRequest request, CancellationToken cancellationToken = default)
    {
        var path = new LearningPath
        {
            Title = request.Title,
            Description = request.Description,
            IconUrl = request.IconUrl,
            PathBadge = request.PathBadge,
            DurationWeeks = request.DurationWeeks,
            IsFree = request.IsFree,
        };

        db.LearningPaths.Add(path);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return await BuildDetailByIdAsync(path.Id, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Path could not be loaded after creation.");
    }

    public async Task<PathDetailResponse?> UpdateAsync(int pathId, UpdatePathRequest request, CancellationToken cancellationToken = default)
    {
        var path = await db.LearningPaths
            .FirstOrDefaultAsync(p => p.Id == pathId, cancellationToken)
            .ConfigureAwait(false);
        if (path is null)
            return null;

        path.Title = request.Title;
        path.Description = request.Description;
        path.IconUrl = request.IconUrl;
        path.PathBadge = request.PathBadge;
        path.DurationWeeks = request.DurationWeeks;
        path.IsFree = request.IsFree;

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return await BuildDetailByIdAsync(pathId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteAsync(int pathId, CancellationToken cancellationToken = default)
    {
        var path = await db.LearningPaths
            .FirstOrDefaultAsync(p => p.Id == pathId, cancellationToken)
            .ConfigureAwait(false);
        if (path is null)
            return false;

        db.LearningPaths.Remove(path);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<PathDetailResponse?> ReplaceCoursesAsync(
        int pathId,
        ReplacePathCoursesRequest request,
        CancellationToken cancellationToken = default)
    {
        var pathExists = await db.LearningPaths
            .AsNoTracking()
            .AnyAsync(p => p.Id == pathId, cancellationToken)
            .ConfigureAwait(false);
        if (!pathExists)
            return null;

        if (request.Courses.Count != request.Courses.Select(c => c.CourseId).Distinct().Count())
            throw new ArgumentException("Duplicate course IDs are not allowed in path ordering.");

        if (request.Courses.Count != request.Courses.Select(c => c.SortOrder).Distinct().Count())
            throw new ArgumentException("Duplicate sort orders are not allowed in path ordering.");

        var requestedCourseIds = request.Courses.Select(c => c.CourseId).Distinct().ToArray();
        var existingCourseIds = await db.Courses
            .AsNoTracking()
            .Where(c => requestedCourseIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var missingCourseIds = requestedCourseIds.Except(existingCourseIds).ToArray();
        if (missingCourseIds.Length > 0)
            throw new KeyNotFoundException($"Some courses were not found: {string.Join(", ", missingCourseIds)}");

        var existingLinks = await db.PathCourses
            .Where(pc => pc.LearningPathId == pathId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        db.PathCourses.RemoveRange(existingLinks);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (request.Courses.Count > 0)
        {
            var newLinks = request.Courses
                .Select(c => new PathCourse
                {
                    LearningPathId = pathId,
                    CourseId = c.CourseId,
                    SortOrder = c.SortOrder,
                })
                .ToList();

            db.PathCourses.AddRange(newLinks);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return await BuildDetailByIdAsync(pathId, cancellationToken).ConfigureAwait(false);
    }

    private async Task<PathDetailResponse?> BuildDetailByIdAsync(int pathId, CancellationToken cancellationToken)
    {
        return await db.LearningPaths
            .AsNoTracking()
            .Where(p => p.Id == pathId)
            .Select(p => new PathDetailResponse(
                p.Id,
                p.Title,
                p.Description,
                p.IconUrl,
                p.PathBadge.HasValue ? p.PathBadge.Value.ToString() : null,
                p.DurationWeeks,
                p.IsFree,
                p.PathCourses
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
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quick_Gen.Contracts.Paths;
using Quick_Gen.Data;
using Quick_Gen.Models;

namespace Quick_Gen.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PathsController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var paths = await db.LearningPaths
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

        return Ok(paths);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var path = await db.LearningPaths
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

        return path is null ? NotFound() : Ok(path);
    }
}


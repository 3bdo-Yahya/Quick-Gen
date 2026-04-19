using Microsoft.EntityFrameworkCore;
using Quick_Gen.Contracts.Courses;
using Quick_Gen.Data;
using Quick_Gen.Models;
using Quick_Gen.Services.IServices;

namespace Quick_Gen.Services;

public sealed class CourseService(ApplicationDbContext db) : ICourseService
{
    //Helpers 
    private static CourseResponse ToResponse(Course c) => new(
        c.Id, c.Title, c.ShortDescription, c.ThumbnailUrl,
        c.CatalogLevel, c.DurationWeeks,
        c.LessonCount, c.IsLocked, c.IsFree);

    //Get all 
    public async Task<IEnumerable<CourseResponse>> GetAllAsync() =>
        await db.Courses
                .AsNoTracking()
                .Select(c => ToResponse(c))  
                .ToListAsync();

    //Get By Id
    public async Task<CourseResponse?> GetByIdAsync(int id)
    {
        var course = await db.Courses.AsNoTracking()
                             .FirstOrDefaultAsync(c => c.Id == id);
        return course is null ? null : ToResponse(course);
    }

    //Create
    public async Task<CourseResponse> CreateAsync(CreateCourseRequest req)
    {
        var course = new Course
        {
            Title = req.Title,
            ShortDescription = req.ShortDescription,
            ThumbnailUrl = req.ThumbnailUrl,
            Difficulty = req.Difficulty,
            CatalogLevel = req.CatalogLevel,
            DurationWeeks = req.DurationWeeks,
            IsLocked = req.IsLocked,
            IsFree = req.IsFree,
            Status = CourseStatus.Draft   
        };

        db.Courses.Add(course);
        await db.SaveChangesAsync();
        return ToResponse(course);
    }

    //UPDATE 
    public async Task<CourseResponse?> UpdateAsync(int id, UpdateCourseRequest req)
    {
        var course = await db.Courses.FindAsync(id);
        if (course is null) return null;

        course.Title = req.Title;
        course.ShortDescription = req.ShortDescription;
        course.ThumbnailUrl = req.ThumbnailUrl;
        course.Difficulty = req.Difficulty;
        course.CatalogLevel = req.CatalogLevel;
        course.DurationWeeks = req.DurationWeeks;
        course.IsLocked = req.IsLocked;
        course.IsFree = req.IsFree;
        course.Status = req.Status;

        await db.SaveChangesAsync();
        return ToResponse(course);
    }

    //DELETE
    public async Task<bool> DeleteAsync(int id)
    {
        var course = await db.Courses.FindAsync(id);
        if (course is null) return false;

        db.Courses.Remove(course);
        await db.SaveChangesAsync();
        return true;
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quick_Gen.Data;
using Quick_Gen.Models;

namespace Quick_Gen.Infrastructure;

public static class IdentitySeeder
{
    public static readonly string AdminRoleName = "Admin";
    public static readonly string StudentRoleName = "Student";

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var context = provider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        await EnsureRoleAsync(roleManager, AdminRoleName).ConfigureAwait(false);
        await EnsureRoleAsync(roleManager, StudentRoleName).ConfigureAwait(false);

        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = provider.GetRequiredService<IConfiguration>();
        var adminEmail = configuration["Seed:AdminEmail"] ?? "admin@quickgen.local";
        var adminPassword = configuration["Seed:AdminPassword"] ?? "yousef@123";

        var admin = await EnsureUserAsync(
            userManager,
            adminEmail,
            adminPassword,
            "Quick Gen Admin",
            cancellationToken).ConfigureAwait(false);
        await EnsureUserInRoleAsync(userManager, admin, AdminRoleName).ConfigureAwait(false);

        var student1 = await EnsureUserAsync(
            userManager,
            "student1@quickgen.local",
            "Student123!",
            "Abdelrahman Yahia",
            cancellationToken).ConfigureAwait(false);
        await EnsureUserInRoleAsync(userManager, student1, StudentRoleName).ConfigureAwait(false);

        var student2 = await EnsureUserAsync(
            userManager,
            "student2@quickgen.local",
            "Student123!",
            "Omar Hassan",
            cancellationToken).ConfigureAwait(false);
        await EnsureUserInRoleAsync(userManager, student2, StudentRoleName).ConfigureAwait(false);

        await SeedDomainDataAsync(context, student1, student2, cancellationToken).ConfigureAwait(false);
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roles, string name)
    {
        if (!await roles.RoleExistsAsync(name).ConfigureAwait(false))
            await roles.CreateAsync(new IdentityRole(name)).ConfigureAwait(false);
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string fullName,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
        if (user is not null)
            return user;

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName,
        };

        var created = await userManager.CreateAsync(user, password).ConfigureAwait(false);
        if (!created.Succeeded)
        {
            var errors = string.Join("; ", created.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to seed user '{email}': {errors}");
        }

        cancellationToken.ThrowIfCancellationRequested();
        return user;
    }

    private static async Task EnsureUserInRoleAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, string role)
    {
        if (!await userManager.IsInRoleAsync(user, role).ConfigureAwait(false))
            await userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
    }

    private static async Task SeedDomainDataAsync(
        ApplicationDbContext db,
        ApplicationUser student1,
        ApplicationUser student2,
        CancellationToken cancellationToken)
    {
        var cSharpBasics = await EnsureCourseAsync(
            db,
            title: "C# Basics",
            shortDescription: "Learn C# fundamentals and object-oriented programming.",
            difficulty: CourseDifficulty.Beginner,
            catalogLevel: 1,
            durationWeeks: 4,
            lessonCount: 4,
            isLocked: false,
            isFree: true,
            status: CourseStatus.Published,
            thumbnailUrl: "https://picsum.photos/seed/csharp/640/360",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var aspNetApis = await EnsureCourseAsync(
            db,
            title: "ASP.NET Core APIs",
            shortDescription: "Build secure REST APIs with ASP.NET Core.",
            difficulty: CourseDifficulty.Intermediate,
            catalogLevel: 2,
            durationWeeks: 6,
            lessonCount: 5,
            isLocked: false,
            isFree: false,
            status: CourseStatus.Published,
            thumbnailUrl: "https://picsum.photos/seed/aspnet/640/360",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var efCore = await EnsureCourseAsync(
            db,
            title: "Entity Framework Core",
            shortDescription: "Master EF Core for data access, migrations, and performance.",
            difficulty: CourseDifficulty.Intermediate,
            catalogLevel: 2,
            durationWeeks: 5,
            lessonCount: 4,
            isLocked: true,
            isFree: false,
            status: CourseStatus.Published,
            thumbnailUrl: "https://picsum.photos/seed/efcore/640/360",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var cleanArchitecture = await EnsureCourseAsync(
            db,
            title: "Clean Architecture Fundamentals",
            shortDescription: "Apply clean architecture and domain-driven principles in .NET.",
            difficulty: CourseDifficulty.Advanced,
            catalogLevel: 3,
            durationWeeks: 8,
            lessonCount: 6,
            isLocked: true,
            isFree: false,
            status: CourseStatus.Published,
            thumbnailUrl: "https://picsum.photos/seed/cleanarch/640/360",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var sqlCourse = await EnsureCourseAsync(
            db,
            title: "SQL for Backend Developers",
            shortDescription: "Write practical SQL queries and optimize relational data access.",
            difficulty: CourseDifficulty.Beginner,
            catalogLevel: 1,
            durationWeeks: 3,
            lessonCount: 3,
            isLocked: false,
            isFree: true,
            status: CourseStatus.Published,
            thumbnailUrl: "https://picsum.photos/seed/sql/640/360",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var draftCourse = await EnsureCourseAsync(
            db,
            title: "Distributed Systems in .NET",
            shortDescription: "Preview advanced distributed patterns for cloud-native systems.",
            difficulty: CourseDifficulty.Advanced,
            catalogLevel: 4,
            durationWeeks: 10,
            lessonCount: 5,
            isLocked: true,
            isFree: false,
            status: CourseStatus.Draft,
            thumbnailUrl: "https://picsum.photos/seed/distributed/640/360",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await EnsureLessonsAsync(db, cSharpBasics.Id, new[]
        {
            ("Introduction to C#", 1, 35),
            ("Data Types and Control Flow", 2, 50),
            ("OOP Basics", 3, 60),
            ("Mini Console Project", 4, 70),
        }, cancellationToken).ConfigureAwait(false);

        await EnsureLessonsAsync(db, aspNetApis.Id, new[]
        {
            ("Web API Fundamentals", 1, 45),
            ("Routing and Controllers", 2, 50),
            ("Validation and DTOs", 3, 45),
            ("Authentication and Authorization", 4, 60),
            ("Production Hardening", 5, 55),
        }, cancellationToken).ConfigureAwait(false);

        await EnsureLessonsAsync(db, efCore.Id, new[]
        {
            ("DbContext and Entities", 1, 40),
            ("Migrations Workflow", 2, 45),
            ("Querying and Tracking", 3, 50),
            ("Performance Tips", 4, 35),
        }, cancellationToken).ConfigureAwait(false);

        await EnsureLessonsAsync(db, cleanArchitecture.Id, new[]
        {
            ("Architecture Layers", 1, 45),
            ("Domain Modeling", 2, 55),
            ("Use Cases and Application Layer", 3, 60),
            ("Infrastructure Boundaries", 4, 45),
            ("Testing Strategy", 5, 50),
            ("Refactoring Workshop", 6, 65),
        }, cancellationToken).ConfigureAwait(false);

        await EnsureLessonsAsync(db, sqlCourse.Id, new[]
        {
            ("SELECT and Filtering", 1, 30),
            ("Joins and Relationships", 2, 40),
            ("Indexing Basics", 3, 35),
        }, cancellationToken).ConfigureAwait(false);

        await EnsureLessonsAsync(db, draftCourse.Id, new[]
        {
            ("Service Communication Patterns", 1, 50),
            ("Resilience and Retries", 2, 40),
        }, cancellationToken).ConfigureAwait(false);

        var backendPath = await EnsurePathAsync(
            db,
            title: "Backend Developer Path",
            description: "A complete backend journey from C# to clean architecture.",
            iconUrl: "https://picsum.photos/seed/backendpath/240/240",
            pathBadge: CourseDifficulty.Intermediate,
            durationWeeks: 12,
            isFree: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var fundamentalsPath = await EnsurePathAsync(
            db,
            title: ".NET Fundamentals Path",
            description: "Foundational path for junior .NET backend learners.",
            iconUrl: "https://picsum.photos/seed/fundamentalspath/240/240",
            pathBadge: CourseDifficulty.Beginner,
            durationWeeks: 10,
            isFree: true,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var dataPath = await EnsurePathAsync(
            db,
            title: "Data Access Specialist Path",
            description: "Focus on SQL and EF Core for backend data handling.",
            iconUrl: "https://picsum.photos/seed/datapath/240/240",
            pathBadge: CourseDifficulty.Intermediate,
            durationWeeks: 8,
            isFree: true,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await ReplacePathCoursesAsync(
            db,
            backendPath.Id,
            (cSharpBasics.Id, 1),
            (aspNetApis.Id, 2),
            (efCore.Id, 3),
            (cleanArchitecture.Id, 4)).ConfigureAwait(false);

        await ReplacePathCoursesAsync(
            db,
            fundamentalsPath.Id,
            (cSharpBasics.Id, 1),
            (sqlCourse.Id, 2),
            (aspNetApis.Id, 3)).ConfigureAwait(false);

        await ReplacePathCoursesAsync(
            db,
            dataPath.Id,
            (sqlCourse.Id, 1),
            (efCore.Id, 2)).ConfigureAwait(false);

        await EnsureEnrollmentAsync(db, student1.Id, cSharpBasics.Id, 100, cancellationToken).ConfigureAwait(false);
        await EnsureEnrollmentAsync(db, student1.Id, aspNetApis.Id, 40, cancellationToken).ConfigureAwait(false);
        await EnsureEnrollmentAsync(db, student2.Id, cSharpBasics.Id, 20, cancellationToken).ConfigureAwait(false);
        await EnsureEnrollmentAsync(db, student2.Id, sqlCourse.Id, 75, cancellationToken).ConfigureAwait(false);

        await EnsureCourseCertificateAsync(
            db,
            student1.Id,
            cSharpBasics.Id,
            recipientDisplayName: student1.FullName ?? student1.Email ?? "Student One",
            courseTitle: cSharpBasics.Title,
            certificateNumber: $"CERT-{DateTime.UtcNow.Year}-SEED0001",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Course> EnsureCourseAsync(
        ApplicationDbContext db,
        string title,
        string shortDescription,
        CourseDifficulty difficulty,
        int catalogLevel,
        int durationWeeks,
        int lessonCount,
        bool isLocked,
        bool isFree,
        CourseStatus status,
        string? thumbnailUrl,
        CancellationToken cancellationToken)
    {
        var existing = await db.Courses.FirstOrDefaultAsync(c => c.Title == title, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            existing = new Course
            {
                Title = title,
                ShortDescription = shortDescription,
                Difficulty = difficulty,
                CatalogLevel = catalogLevel,
                DurationWeeks = durationWeeks,
                LessonCount = lessonCount,
                IsLocked = isLocked,
                IsFree = isFree,
                Status = status,
                ThumbnailUrl = thumbnailUrl,
            };
            db.Courses.Add(existing);
        }
        else
        {
            existing.ShortDescription = shortDescription;
            existing.Difficulty = difficulty;
            existing.CatalogLevel = catalogLevel;
            existing.DurationWeeks = durationWeeks;
            existing.LessonCount = lessonCount;
            existing.IsLocked = isLocked;
            existing.IsFree = isFree;
            existing.Status = status;
            existing.ThumbnailUrl = thumbnailUrl;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return existing;
    }

    private static async Task EnsureLessonsAsync(
        ApplicationDbContext db,
        int courseId,
        IEnumerable<(string title, int sortOrder, int durationMinutes)> lessons,
        CancellationToken cancellationToken)
    {
        foreach (var lesson in lessons)
        {
            var existing = await db.Lessons.FirstOrDefaultAsync(
                l => l.CourseId == courseId && l.SortOrder == lesson.sortOrder,
                cancellationToken).ConfigureAwait(false);

            if (existing is null)
            {
                db.Lessons.Add(new Lesson
                {
                    CourseId = courseId,
                    Title = lesson.title,
                    SortOrder = lesson.sortOrder,
                    DurationMinutes = lesson.durationMinutes,
                    ContentUrl = $"https://learn.quickgen.local/courses/{courseId}/lessons/{lesson.sortOrder}",
                });
            }
            else
            {
                existing.Title = lesson.title;
                existing.DurationMinutes = lesson.durationMinutes;
                existing.ContentUrl = $"https://learn.quickgen.local/courses/{courseId}/lessons/{lesson.sortOrder}";
            }
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task<LearningPath> EnsurePathAsync(
        ApplicationDbContext db,
        string title,
        string description,
        string? iconUrl,
        CourseDifficulty? pathBadge,
        int durationWeeks,
        bool isFree,
        CancellationToken cancellationToken)
    {
        var existing = await db.LearningPaths
            .FirstOrDefaultAsync(p => p.Title == title, cancellationToken)
            .ConfigureAwait(false);

        if (existing is null)
        {
            existing = new LearningPath
            {
                Title = title,
                Description = description,
                IconUrl = iconUrl,
                PathBadge = pathBadge,
                DurationWeeks = durationWeeks,
                IsFree = isFree,
            };
            db.LearningPaths.Add(existing);
        }
        else
        {
            existing.Description = description;
            existing.IconUrl = iconUrl;
            existing.PathBadge = pathBadge;
            existing.DurationWeeks = durationWeeks;
            existing.IsFree = isFree;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return existing;
    }

    private static async Task ReplacePathCoursesAsync(
        ApplicationDbContext db,
        int pathId,
        params (int courseId, int sortOrder)[] ordering)
    {
        var existing = await db.PathCourses
            .Where(pc => pc.LearningPathId == pathId)
            .ToListAsync()
            .ConfigureAwait(false);

        db.PathCourses.RemoveRange(existing);
        await db.SaveChangesAsync().ConfigureAwait(false);

        if (ordering.Length == 0)
            return;

        db.PathCourses.AddRange(ordering.Select(x => new PathCourse
        {
            LearningPathId = pathId,
            CourseId = x.courseId,
            SortOrder = x.sortOrder,
        }));

        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    private static async Task EnsureEnrollmentAsync(
        ApplicationDbContext db,
        string userId,
        int courseId,
        int progressPercent,
        CancellationToken cancellationToken)
    {
        var enrollment = await db.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId, cancellationToken)
            .ConfigureAwait(false);

        if (enrollment is null)
        {
            enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                EnrolledAt = DateTimeOffset.UtcNow.AddDays(-14),
                ProgressPercent = Math.Clamp(progressPercent, 0, 100),
            };
            db.Enrollments.Add(enrollment);
        }
        else
        {
            enrollment.ProgressPercent = Math.Clamp(progressPercent, 0, 100);
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task EnsureCourseCertificateAsync(
        ApplicationDbContext db,
        string userId,
        int courseId,
        string recipientDisplayName,
        string courseTitle,
        string certificateNumber,
        CancellationToken cancellationToken)
    {
        var existing = await db.Certificates
            .FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId, cancellationToken)
            .ConfigureAwait(false);

        if (existing is not null)
            return;

        var certificateNumberTaken = await db.Certificates
            .AnyAsync(c => c.CertificateNumber == certificateNumber, cancellationToken)
            .ConfigureAwait(false);

        db.Certificates.Add(new Certificate
        {
            CertificateNumber = certificateNumberTaken
                ? $"CERT-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}"
                : certificateNumber,
            UserId = userId,
            CourseId = courseId,
            IssuedAt = DateTimeOffset.UtcNow.AddDays(-1),
            RecipientDisplayName = recipientDisplayName,
            CourseTitle = courseTitle,
        });

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

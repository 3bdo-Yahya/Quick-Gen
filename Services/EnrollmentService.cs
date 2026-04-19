using Microsoft.EntityFrameworkCore;
using Quick_Gen.Contracts.Enrollments;
using Quick_Gen.Data;
using Quick_Gen.Models;
using Quick_Gen.Services.IServices;

namespace Quick_Gen.Services;

public sealed class EnrollmentService(ApplicationDbContext db) : IEnrollmentService
{
    public async Task<EnrollmentOperationResult> EnrollAsync(
        string userId,
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var courseExists = await db.Courses
            .AsNoTracking()
            .AnyAsync(c => c.Id == courseId, cancellationToken)
            .ConfigureAwait(false);
        if (!courseExists)
            return new EnrollmentOperationResult(EnrollmentOperationStatus.CourseNotFound, null);

        var alreadyEnrolled = await db.Enrollments
            .AsNoTracking()
            .AnyAsync(e => e.UserId == userId && e.CourseId == courseId, cancellationToken)
            .ConfigureAwait(false);
        if (alreadyEnrolled)
            return new EnrollmentOperationResult(EnrollmentOperationStatus.AlreadyEnrolled, null);

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTimeOffset.UtcNow,
            ProgressPercent = 0,
        };

        db.Enrollments.Add(enrollment);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new EnrollmentOperationResult(
            EnrollmentOperationStatus.Success,
            ToResponse(enrollment));
    }

    public async Task<EnrollmentOperationResult> UpdateProgressAsync(
        string userId,
        int courseId,
        int progressPercent,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await db.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId, cancellationToken)
            .ConfigureAwait(false);
        if (enrollment is null)
        {
            var courseExists = await db.Courses
                .AsNoTracking()
                .AnyAsync(c => c.Id == courseId, cancellationToken)
                .ConfigureAwait(false);

            return courseExists
                ? new EnrollmentOperationResult(EnrollmentOperationStatus.EnrollmentNotFound, null)
                : new EnrollmentOperationResult(EnrollmentOperationStatus.CourseNotFound, null);
        }

        enrollment.ProgressPercent = Math.Clamp(progressPercent, 0, 100);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new EnrollmentOperationResult(
            EnrollmentOperationStatus.Success,
            ToResponse(enrollment));
    }

    private static EnrollmentResponse ToResponse(Enrollment enrollment)
    {
        return new EnrollmentResponse(
            enrollment.Id,
            enrollment.CourseId,
            enrollment.UserId,
            enrollment.ProgressPercent,
            enrollment.EnrolledAt);
    }
}


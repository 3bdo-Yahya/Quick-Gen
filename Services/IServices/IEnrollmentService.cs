using Quick_Gen.Contracts.Enrollments;

namespace Quick_Gen.Services.IServices;

public interface IEnrollmentService
{
    Task<EnrollmentOperationResult> EnrollAsync(string userId, int courseId, CancellationToken cancellationToken = default);

    Task<EnrollmentOperationResult> UpdateProgressAsync(
        string userId,
        int courseId,
        int progressPercent,
        CancellationToken cancellationToken = default);
}

public enum EnrollmentOperationStatus
{
    Success = 0,
    CourseNotFound = 1,
    EnrollmentNotFound = 2,
    AlreadyEnrolled = 3,
}

public sealed record EnrollmentOperationResult(
    EnrollmentOperationStatus Status,
    EnrollmentResponse? Enrollment);


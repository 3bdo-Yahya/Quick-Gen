namespace Quick_Gen.Contracts.Enrollments;

public sealed record EnrollmentResponse(
    int Id,
    int CourseId,
    string UserId,
    int ProgressPercent,
    DateTimeOffset EnrolledAt
);


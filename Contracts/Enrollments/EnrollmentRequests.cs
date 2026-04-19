using System.ComponentModel.DataAnnotations;

namespace Quick_Gen.Contracts.Enrollments;

public sealed class CreateEnrollmentRequest
{
    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }
}

public sealed class UpdateEnrollmentProgressRequest
{
    public int ProgressPercent { get; set; }
}


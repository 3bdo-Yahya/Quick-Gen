using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quick_Gen.Contracts.Enrollments;
using Quick_Gen.Services.IServices;

namespace Quick_Gen.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class EnrollmentsController(IEnrollmentService enrollmentService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result = await enrollmentService
            .EnrollAsync(userId, request.CourseId, cancellationToken)
            .ConfigureAwait(false);

        return result.Status switch
        {
            EnrollmentOperationStatus.CourseNotFound => NotFound(new { message = "Course not found." }),
            EnrollmentOperationStatus.AlreadyEnrolled => Conflict(new { message = "User is already enrolled in this course." }),
            EnrollmentOperationStatus.Success => Ok(result.Enrollment),
            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }

    [HttpPatch("{courseId:int}/progress")]
    public async Task<IActionResult> UpdateProgress(
        [FromRoute] int courseId,
        [FromBody] UpdateEnrollmentProgressRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result = await enrollmentService
            .UpdateProgressAsync(userId, courseId, request.ProgressPercent, cancellationToken)
            .ConfigureAwait(false);

        return result.Status switch
        {
            EnrollmentOperationStatus.CourseNotFound => NotFound(new { message = "Course not found." }),
            EnrollmentOperationStatus.EnrollmentNotFound => NotFound(new { message = "Enrollment was not found for this course." }),
            EnrollmentOperationStatus.Success => Ok(result.Enrollment),
            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }
}


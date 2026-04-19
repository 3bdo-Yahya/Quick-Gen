using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quick_Gen.Services.IServices;
using System.Security.Claims;

namespace Quick_Gen.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CertificatesController(ICertificateService certService) : ControllerBase
{
    // POST api/certificates/generate/course/5
    [HttpPost("generate/course/{courseId:int}")]
    [Authorize]
    public async Task<IActionResult> GenerateForCourse(int courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        try
        {
            var cert = await certService.GenerateForCourseAsync(userId, courseId);
            return Ok(cert);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    // GET api/certificates/my
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> MyCertificates()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        return Ok(await certService.GetUserCertificatesAsync(userId));
    }

    // GET api/certificates/verify/CERT-2026-XXXXXXXX
    [HttpGet("verify/{number}")]
    [AllowAnonymous]
    public async Task<IActionResult> Verify(string number)
    {
        var cert = await certService.GetByNumberAsync(number);
        return cert is null ? NotFound("Invalid certificate.") : Ok(cert);
    }

    // GET api/certificates/CERT-2026-XXXXXXXX/pdf
    [HttpGet("{number}/pdf")]
    [Authorize]
    public async Task<IActionResult> DownloadPdf(string number)
    {
        try
        {
            var pdf = await certService.GeneratePdfAsync(number);
            return File(pdf, "application/pdf", $"{number}.pdf");
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }
}
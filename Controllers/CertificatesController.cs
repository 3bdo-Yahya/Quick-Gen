using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quick_Gen.Services.IServices;
using System.Security.Claims;

namespace Quick_Gen.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CertificatesController(ICertificateService certService) : ControllerBase
{
    private string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // POST api/certificates/generate/course/5
    [HttpPost("generate/course/{courseId:int}")]
    [Authorize]
    public async Task<IActionResult> GenerateForCourse(int courseId)
    {
        try
        {
            var cert = await certService.GenerateForCourseAsync(UserId, courseId);
            return Ok(cert);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    // GET api/certificates/my
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> MyCertificates() =>
        Ok(await certService.GetUserCertificatesAsync(UserId));

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
    [AllowAnonymous]
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
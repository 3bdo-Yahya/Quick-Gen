using Microsoft.EntityFrameworkCore;
using Quick_Gen.Contracts.Certificates;
using Quick_Gen.Data;
using Quick_Gen.Infrastructure;
using Quick_Gen.Models;
using Quick_Gen.Services.IServices;

namespace Quick_Gen.Services;

public sealed class CertificateService(
    ApplicationDbContext db,
    IConfiguration config) : ICertificateService
{
    //Helper
    private CertificateResponse ToResponse(Certificate c) => new(
        c.Id,
        c.CertificateNumber,
        c.RecipientDisplayName ?? string.Empty,
        c.CourseTitle ?? string.Empty,
        c.IssuedAt.ToString("MMMM dd, yyyy"),
        $"{config["App:BaseUrl"]}/api/certificates/verify/{c.CertificateNumber}"
    );

    private static string GenerateCertificateNumber() =>
        $"CERT-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

    // Generate
    public async Task<CertificateResponse> GenerateForCourseAsync(string userId, int courseId)
    {
        
        var course = await db.Courses.FindAsync(courseId)
            ?? throw new KeyNotFoundException("Course not found.");

      
        var user = await db.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

     
        var enrollment = await db.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId)
            ?? throw new InvalidOperationException("User is not enrolled in this course.");

 
        if (enrollment.ProgressPercent < 100)
            throw new InvalidOperationException("Course not completed yet.");

    
        var existing = await db.Certificates
            .FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId);

        if (existing is not null)
            return ToResponse(existing);

 
        var certificate = new Certificate
        {
            CertificateNumber = GenerateCertificateNumber(),
            UserId = userId,
            CourseId = courseId,
            IssuedAt = DateTimeOffset.UtcNow,
            RecipientDisplayName = user.FullName,
            CourseTitle = course.Title
        };

        db.Certificates.Add(certificate);
        await db.SaveChangesAsync();

        return ToResponse(certificate);
    }

    //Verify
    public async Task<CertificateResponse?> GetByNumberAsync(string certificateNumber)
    {
        var cert = await db.Certificates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CertificateNumber == certificateNumber);

        return cert is null ? null : ToResponse(cert);
    }

    //My Certificates
    public async Task<IEnumerable<CertificateResponse>> GetUserCertificatesAsync(string userId)
    {
        return await db.Certificates
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => ToResponse(c))
            .ToListAsync();
    }

    //PDF
    public async Task<byte[]> GeneratePdfAsync(string certificateNumber)
    {
        var cert = await db.Certificates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CertificateNumber == certificateNumber)
            ?? throw new KeyNotFoundException("Certificate not found.");

        return CertificatePdfGenerator.Generate(cert);
    }
}
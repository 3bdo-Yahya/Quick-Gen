namespace Quick_Gen.Contracts.Certificates;

public sealed record CertificateResponse(
    int Id,
    string CertificateNumber,
    string RecipientDisplayName,
    string CourseTitle,
    string IssuedAt,
    string? VerifyUrl
);
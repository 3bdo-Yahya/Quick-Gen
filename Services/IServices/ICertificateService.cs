using Quick_Gen.Contracts.Certificates;

namespace Quick_Gen.Services.IServices
{
    public interface ICertificateService
    {
        Task<CertificateResponse> GenerateForCourseAsync(string userId, int courseId);
        Task<CertificateResponse?> GetByNumberAsync(string certificateNumber);
        Task<IEnumerable<CertificateResponse>> GetUserCertificatesAsync(string userId);
        Task<byte[]> GeneratePdfAsync(string certificateNumber);
    }
}

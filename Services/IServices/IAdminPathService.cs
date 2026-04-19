using Quick_Gen.Contracts.Paths;

namespace Quick_Gen.Services.IServices;

public interface IAdminPathService
{
    Task<PathDetailResponse> CreateAsync(CreatePathRequest request, CancellationToken cancellationToken = default);
    Task<PathDetailResponse?> UpdateAsync(int pathId, UpdatePathRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int pathId, CancellationToken cancellationToken = default);
    Task<PathDetailResponse?> ReplaceCoursesAsync(int pathId, ReplacePathCoursesRequest request, CancellationToken cancellationToken = default);
}


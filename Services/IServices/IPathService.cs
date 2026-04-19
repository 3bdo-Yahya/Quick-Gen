using Quick_Gen.Contracts.Paths;

namespace Quick_Gen.Services.IServices;

public interface IPathService
{
    Task<IReadOnlyList<PathListItemResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PathDetailResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}


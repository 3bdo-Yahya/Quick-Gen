using Quick_Gen.Models;

namespace Quick_Gen.Services.IServices;

public interface ITokenService
{
    Task<(string Token, DateTimeOffset ExpiresAtUtc)> CreateTokenAsync(ApplicationUser user,
        CancellationToken cancellationToken = default);
}

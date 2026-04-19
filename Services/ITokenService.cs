using Quick_Gen.Models;

namespace Quick_Gen.Services;

public interface ITokenService
{
    Task<(string Token, DateTimeOffset ExpiresAtUtc)> CreateTokenAsync(ApplicationUser user,
        CancellationToken cancellationToken = default);
}

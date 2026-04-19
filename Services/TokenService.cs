using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quick_Gen.Configuration;
using Quick_Gen.Models;

namespace Quick_Gen.Services;

public sealed class TokenService : ITokenService
{
    private readonly JwtOptions _jwt;
    private readonly UserManager<ApplicationUser> _users;

    public TokenService(IOptions<JwtOptions> jwtOptions, UserManager<ApplicationUser> users)
    {
        _jwt = jwtOptions.Value;
        _users = users;
    }

    public async Task<(string Token, DateTimeOffset ExpiresAtUtc)> CreateTokenAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_jwt.Key) || _jwt.Key.Length < 32)
            throw new InvalidOperationException("Jwt:Key must be at least 32 characters. Set user-secrets or appsettings for Development.");

        var roles = await _users.GetRolesAsync(user).ConfigureAwait(false);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
        };

        if (!string.IsNullOrEmpty(user.Email))
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));

        if (!string.IsNullOrEmpty(user.FullName))
            claims.Add(new Claim(ClaimTypes.Name, user.FullName));

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var keyBytes = Encoding.UTF8.GetBytes(_jwt.Key);
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(_jwt.ExpiresHours);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var serialized = new JwtSecurityTokenHandler().WriteToken(token);
        return (serialized, new DateTimeOffset(expires, TimeSpan.Zero));
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Quick_Gen.Contracts;
using Quick_Gen.Infrastructure;
using Quick_Gen.Models;
using Quick_Gen.Services.IServices;

namespace Quick_Gen.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokens;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        ITokenService tokens)
    {
        _userManager = userManager;
        _tokens = tokens;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
        if (existing is not null)
            return Conflict(new { message = "Email is already registered." });

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            EmailConfirmed = true,
        };

        var result = await _userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message = errors });
        }

        await _userManager.AddToRoleAsync(user, IdentitySeeder.StudentRoleName).ConfigureAwait(false);

        var token = await _tokens.CreateTokenAsync(user, cancellationToken).ConfigureAwait(false);
        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

        return Ok(new AuthResponse
        {
            Token = token.Token,
            ExpiresAtUtc = token.ExpiresAtUtc,
            User = ToSummary(user, roles),
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
        if (user is null)
            return Unauthorized(new { message = "Invalid email or password." });

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password).ConfigureAwait(false);
        if (!passwordValid)
            return Unauthorized(new { message = "Invalid email or password." });

        var token = await _tokens.CreateTokenAsync(user, cancellationToken).ConfigureAwait(false);
        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

        return Ok(new AuthResponse
        {
            Token = token.Token,
            ExpiresAtUtc = token.ExpiresAtUtc,
            User = ToSummary(user, roles),
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserSummary>> Me(CancellationToken cancellationToken)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
        if (user is null)
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        return Ok(ToSummary(user, roles));
    }

    private static UserSummary ToSummary(ApplicationUser user, IList<string> roles)
    {
        return new UserSummary
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Roles = roles.ToList(),
        };
    }
}

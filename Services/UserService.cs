using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quick_Gen.Contracts.Users;
using Quick_Gen.Models;
using Quick_Gen.Services.IServices;

namespace Quick_Gen.Services;

public sealed class UserService(
    UserManager<ApplicationUser> userManager) : IUserService
{
    //Helper 
    private static async Task<UserResponse> ToResponse(
        ApplicationUser u, UserManager<ApplicationUser> um)
    {
        var roles = await um.GetRolesAsync(u);
        return new UserResponse(
            u.Id,
            u.FullName ?? string.Empty,
            u.Email ?? string.Empty,
            roles.FirstOrDefault() ?? "Student",
            u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow,
            u.CreatedAt.ToString("yyyy-MM-dd")
        );
    }

    //GET All
    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = await userManager.Users.AsNoTracking().ToListAsync();
        var result = new List<UserResponse>();
        foreach (var u in users)
            result.Add(await ToResponse(u, userManager));
        return result;
    }

    //GET By Id
    public async Task<UserResponse?> GetByIdAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        return user is null ? null : await ToResponse(user, userManager);
    }

    //CREATE
    public async Task<UserResponse> CreateAsync(CreateUserRequest req)
    {
        var user = new ApplicationUser
        {
            FullName = req.FullName,
            Email = req.Email,
            UserName = req.Email,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = await userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, "Student");
        return await ToResponse(user, userManager);
    }

    //UPDATE 
    public async Task<UserResponse?> UpdateAsync(string id, UpdateUserRequest req)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return null;

        user.FullName = req.FullName;
        user.Email = req.Email;
        user.UserName = req.Email;

        await userManager.UpdateAsync(user);
        return await ToResponse(user, userManager);
    }

    // DELETE 
    public async Task<bool> DeleteAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return false;
        await userManager.DeleteAsync(user);
        return true;
    }
}
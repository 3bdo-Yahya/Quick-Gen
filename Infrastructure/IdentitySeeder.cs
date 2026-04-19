using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quick_Gen.Data;
using Quick_Gen.Models;

namespace Quick_Gen.Infrastructure;

public static class IdentitySeeder
{
    public static readonly string AdminRoleName = "Admin";
    public static readonly string StudentRoleName = "Student";

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var context = provider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        await EnsureRoleAsync(roleManager, AdminRoleName).ConfigureAwait(false);
        await EnsureRoleAsync(roleManager, StudentRoleName).ConfigureAwait(false);

        var configuration = provider.GetRequiredService<IConfiguration>();
        var adminEmail = configuration["Seed:AdminEmail"];
        var adminPassword = configuration["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            return;

        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var existing = await userManager.FindByEmailAsync(adminEmail).ConfigureAwait(false);
        if (existing is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "Site Admin",
        };

        var created = await userManager.CreateAsync(admin, adminPassword).ConfigureAwait(false);
        if (!created.Succeeded)
            return;

        await userManager.AddToRoleAsync(admin, AdminRoleName).ConfigureAwait(false);
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roles, string name)
    {
        if (!await roles.RoleExistsAsync(name).ConfigureAwait(false))
            await roles.CreateAsync(new IdentityRole(name)).ConfigureAwait(false);
    }
}

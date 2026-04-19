using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quick_Gen.Configuration;
using Quick_Gen.Data;
using Quick_Gen.Models;
using Quick_Gen.Services;
using Quick_Gen.Services.IServices;

namespace Quick_Gen.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuickGenPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'DefaultConnection' is missing. Check appsettings or user secrets.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IPathService, PathService>();
        services.AddScoped<IAdminPathService, AdminPathService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        services.AddScoped<ICertificateService , CertificateService>();

        return services;
    }

    public static IServiceCollection AddQuickGenJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var key = jwtSection["Key"];

        if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
            throw new InvalidOperationException(
                "Jwt:Key must be configured (min 32 chars). Use dotnet user-secrets set \"Jwt:Key\" \"...\" or appsettings.Development.json.");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.FromMinutes(1),
                };
            });

        services.AddAuthorization();

        return services;
    }
}

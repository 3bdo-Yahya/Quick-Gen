using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quick_Gen.Models;

namespace Quick_Gen.Data;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LearningPath> LearningPaths => Set<LearningPath>();
    public DbSet<PathCourse> PathCourses => Set<PathCourse>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Certificate> Certificates => Set<Certificate>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Course>(entity =>
        {
            entity.Property(c => c.Title).HasMaxLength(500);
            entity.Property(c => c.ShortDescription).HasMaxLength(4000);
            entity.Property(c => c.ThumbnailUrl).HasMaxLength(2000);
        });

        builder.Entity<Lesson>(entity =>
        {
            entity.Property(l => l.Title).HasMaxLength(500);
            entity.Property(l => l.ContentUrl).HasMaxLength(2000);
            entity.HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<LearningPath>(entity =>
        {
            entity.Property(p => p.Title).HasMaxLength(500);
            entity.Property(p => p.Description).HasMaxLength(8000);
            entity.Property(p => p.IconUrl).HasMaxLength(2000);
            entity.ToTable("LearningPaths");
        });

        builder.Entity<PathCourse>(entity =>
        {
            entity.HasIndex(pc => new { pc.LearningPathId, pc.SortOrder }).IsUnique();

            entity.HasOne(pc => pc.LearningPath)
                .WithMany(p => p.PathCourses)
                .HasForeignKey(pc => pc.LearningPathId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pc => pc.Course)
                .WithMany(c => c.PathLinks)
                .HasForeignKey(pc => pc.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Enrollment>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Certificate>(entity =>
        {
            entity.HasIndex(c => c.CertificateNumber).IsUnique();

            entity.Property(c => c.CertificateNumber).HasMaxLength(64);
            entity.Property(c => c.RecipientDisplayName).HasMaxLength(256);
            entity.Property(c => c.CourseTitle).HasMaxLength(500);

            entity.HasOne(c => c.User)
                .WithMany(u => u.Certificates)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Course)
                .WithMany(course => course.Certificates)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.LearningPath)
                .WithMany(p => p.Certificates)
                .HasForeignKey(c => c.LearningPathId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Certificate_HasCourseOrPath",
                    "([CourseId] IS NOT NULL) OR ([LearningPathId] IS NOT NULL)"));
        });
    }
}

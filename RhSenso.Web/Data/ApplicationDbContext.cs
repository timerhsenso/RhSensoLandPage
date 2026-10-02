using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RhSenso.Web.Models;
using RhSenso.Web.Models.Analytics;

namespace RhSenso.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    // Contato e currículos
    public DbSet<ContactRequest> ContactRequests => Set<ContactRequest>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

    // Analytics
    public DbSet<AnalyticsVisitor> AnalyticsVisitors => Set<AnalyticsVisitor>();
    public DbSet<AnalyticsSession> AnalyticsSessions => Set<AnalyticsSession>();
    public DbSet<AnalyticsEvent> AnalyticsEvents => Set<AnalyticsEvent>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // =========================================================
        // ANALYTICS - VISITANTES
        // =========================================================

        builder.Entity<AnalyticsVisitor>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.VisitorKey)
                .IsUnique();

            entity.Property(x => x.VisitorKey)
                .IsRequired();

            entity.Property(x => x.FirstVisitAt)
                .IsRequired();

            entity.Property(x => x.LastVisitAt)
                .IsRequired();
        });

        // =========================================================
        // ANALYTICS - SESSÕES
        // =========================================================

        builder.Entity<AnalyticsSession>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.SessionKey)
                .IsUnique();

            entity.Property(x => x.SessionKey)
                .IsRequired();

            entity.Property(x => x.LandingPage)
                .HasMaxLength(1000);

            entity.Property(x => x.Referrer)
                .HasMaxLength(2000);

            entity.Property(x => x.UtmSource)
                .HasMaxLength(200);

            entity.Property(x => x.UtmMedium)
                .HasMaxLength(200);

            entity.Property(x => x.UtmCampaign)
                .HasMaxLength(300);

            entity.Property(x => x.IpHash)
                .HasMaxLength(128);

            entity.Property(x => x.UserAgent)
                .HasMaxLength(2000);

            entity.Property(x => x.DeviceType)
                .HasMaxLength(50);

            entity.Property(x => x.Browser)
                .HasMaxLength(100);

            entity.Property(x => x.OperatingSystem)
                .HasMaxLength(100);

            entity.HasOne(x => x.Visitor)
                .WithMany(x => x.Sessions)
                .HasForeignKey(x => x.VisitorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =========================================================
        // ANALYTICS - EVENTOS
        // =========================================================

        builder.Entity<AnalyticsEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.EventType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.EventName)
                .HasMaxLength(300);

            entity.Property(x => x.PageUrl)
                .HasMaxLength(2000);

            entity.Property(x => x.TargetUrl)
                .HasMaxLength(2000);

            // Índices para as consultas do dashboard
            entity.HasIndex(x => x.CreatedAt);
            entity.HasIndex(x => x.EventType);

            entity.HasOne(x => x.Session)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
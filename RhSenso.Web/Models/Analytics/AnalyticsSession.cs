namespace RhSenso.Web.Models.Analytics;

public class AnalyticsSession
{
    public long Id { get; set; }

    public Guid SessionKey { get; set; }

    public long VisitorId { get; set; }

    public AnalyticsVisitor Visitor { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime LastActivityAt { get; set; }

    public string? LandingPage { get; set; }

    public string? Referrer { get; set; }

    public string? UtmSource { get; set; }

    public string? UtmMedium { get; set; }

    public string? UtmCampaign { get; set; }

    // Não vamos guardar o IP puro.
    // Posteriormente gravaremos apenas um hash.
    public string? IpHash { get; set; }

    public string? UserAgent { get; set; }

    public string? DeviceType { get; set; }

    public string? Browser { get; set; }

    public string? OperatingSystem { get; set; }

    public ICollection<AnalyticsEvent> Events { get; set; }
        = new List<AnalyticsEvent>();
}
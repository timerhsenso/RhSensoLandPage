namespace RhSenso.Web.ViewModels.Analytics;

public class AnalyticsDashboardViewModel
{
    // Período do relatório
    public int Days { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Cards principais
    public int Visitors { get; set; }
    public int Sessions { get; set; }
    public int PageViews { get; set; }
    public int Clicks { get; set; }

    // Indicadores adicionais
    public int ReturningVisitors { get; set; }
    public int NewVisitors { get; set; }

    public double PagesPerSession { get; set; }
    public double ClicksPerSession { get; set; }

    // Rankings
    public List<AnalyticsRankingItem> TopPages { get; set; } = new();

    public List<AnalyticsRankingItem> TopClicks { get; set; } = new();

    public List<AnalyticsRankingItem> TopReferrers { get; set; } = new();

    public List<AnalyticsRankingItem> TopSources { get; set; } = new();

    // Últimas sessões
    public List<AnalyticsRecentSession> RecentSessions { get; set; } = new();
}


public class AnalyticsRankingItem
{
    public string Name { get; set; } = string.Empty;

    public int Count { get; set; }
}


public class AnalyticsRecentSession
{
    public DateTime StartedAt { get; set; }

    public DateTime LastActivityAt { get; set; }

    public string LandingPage { get; set; } = string.Empty;

    public string Referrer { get; set; } = string.Empty;

    public string DeviceType { get; set; } = string.Empty;

    public int PageViews { get; set; }

    public int Clicks { get; set; }
}
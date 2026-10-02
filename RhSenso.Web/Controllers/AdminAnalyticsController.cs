using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSenso.Web.Data;
using RhSenso.Web.ViewModels.Analytics;

namespace RhSenso.Web.Controllers;

[Authorize(Roles = "CEO")]
[Route("admin/analytics")]
public class AdminAnalyticsController(
    ApplicationDbContext db) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int days = 30)
    {
        // Evita períodos absurdos ou inválidos.
        if (days is not (7 or 30 or 90 or 365))
        {
            days = 30;
        }

        var now = DateTime.UtcNow;
        var startDate = now.AddDays(-days);

        // =========================================================
        // VISITANTES
        // =========================================================

        var visitors = await db.AnalyticsSessions
            .AsNoTracking()
            .Where(x => x.StartedAt >= startDate)
            .Select(x => x.VisitorId)
            .Distinct()
            .CountAsync();


        // =========================================================
        // SESSÕES
        // =========================================================

        var sessions = await db.AnalyticsSessions
            .AsNoTracking()
            .CountAsync(x =>
                x.StartedAt >= startDate);


        // =========================================================
        // PAGE VIEWS
        // =========================================================

        var pageViews = await db.AnalyticsEvents
            .AsNoTracking()
            .CountAsync(x =>
                x.CreatedAt >= startDate &&
                x.EventType == "page_view");


        // =========================================================
        // CLIQUES
        // =========================================================

        var clicks = await db.AnalyticsEvents
            .AsNoTracking()
            .CountAsync(x =>
                x.CreatedAt >= startDate &&
                x.EventType == "click");


        // =========================================================
        // VISITANTES NOVOS
        // =========================================================

        var newVisitors = await db.AnalyticsVisitors
            .AsNoTracking()
            .CountAsync(x =>
                x.FirstVisitAt >= startDate);


        // =========================================================
        // VISITANTES RECORRENTES
        // =========================================================

        var returningVisitors = await db.AnalyticsVisitors
            .AsNoTracking()
            .CountAsync(x =>
                x.LastVisitAt >= startDate &&
                x.TotalSessions > 1);


        // =========================================================
        // PÁGINAS MAIS VISITADAS
        // =========================================================

        var topPages = await db.AnalyticsEvents
            .AsNoTracking()
            .Where(x =>
                x.CreatedAt >= startDate &&
                x.EventType == "page_view" &&
                x.PageUrl != null)
            .GroupBy(x => x.PageUrl!)
            .Select(g => new AnalyticsRankingItem
            {
                Name = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();


        // =========================================================
        // LINKS / BOTÕES MAIS CLICADOS
        // =========================================================

        var topClicks = await db.AnalyticsEvents
            .AsNoTracking()
            .Where(x =>
                x.CreatedAt >= startDate &&
                x.EventType == "click" &&
                x.EventName != null)
            .GroupBy(x => x.EventName!)
            .Select(g => new AnalyticsRankingItem
            {
                Name = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();


        // =========================================================
        // ORIGEM / REFERRER
        // =========================================================

        var topReferrers = await db.AnalyticsSessions
            .AsNoTracking()
            .Where(x =>
                x.StartedAt >= startDate &&
                x.Referrer != null &&
                x.Referrer != "")
            .GroupBy(x => x.Referrer!)
            .Select(g => new AnalyticsRankingItem
            {
                Name = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();


        // =========================================================
        // UTM SOURCE
        // =========================================================

        var topSources = await db.AnalyticsSessions
            .AsNoTracking()
            .Where(x =>
                x.StartedAt >= startDate &&
                x.UtmSource != null &&
                x.UtmSource != "")
            .GroupBy(x => x.UtmSource!)
            .Select(g => new AnalyticsRankingItem
            {
                Name = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();


        // =========================================================
        // ÚLTIMAS SESSÕES
        // =========================================================

        var recentSessions = await db.AnalyticsSessions
            .AsNoTracking()
            .Where(x =>
                x.StartedAt >= startDate)
            .OrderByDescending(x => x.StartedAt)
            .Take(20)
            .Select(x => new AnalyticsRecentSession
            {
                StartedAt = x.StartedAt,

                LastActivityAt =
                    x.LastActivityAt,

                LandingPage =
                    x.LandingPage ?? "/",

                Referrer =
                    x.Referrer ?? "Acesso direto",

                DeviceType =
                    x.DeviceType ?? "Não identificado",

                PageViews =
                    x.Events.Count(e =>
                        e.EventType == "page_view"),

                Clicks =
                    x.Events.Count(e =>
                        e.EventType == "click")
            })
            .ToListAsync();


        // =========================================================
        // VIEW MODEL
        // =========================================================

        var model =
            new AnalyticsDashboardViewModel
            {
                Days = days,

                StartDate = startDate,

                EndDate = now,

                Visitors = visitors,

                Sessions = sessions,

                PageViews = pageViews,

                Clicks = clicks,

                NewVisitors = newVisitors,

                ReturningVisitors =
                    returningVisitors,

                PagesPerSession =
                    sessions == 0
                        ? 0
                        : Math.Round(
                            (double)pageViews / sessions,
                            2),

                ClicksPerSession =
                    sessions == 0
                        ? 0
                        : Math.Round(
                            (double)clicks / sessions,
                            2),

                TopPages = topPages,

                TopClicks = topClicks,

                TopReferrers =
                    topReferrers,

                TopSources =
                    topSources,

                RecentSessions =
                    recentSessions
            };


        return View(model);
    }
}
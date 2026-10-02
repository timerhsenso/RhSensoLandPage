using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSenso.Web.Data;
using RhSenso.Web.Models.Analytics;
using RhSenso.Web.Models.Analytics.Dto;
using System.Security.Cryptography;
using System.Text;

namespace RhSenso.Web.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController(
    ApplicationDbContext db,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost("session")]
    public async Task<IActionResult> StartSession(
        [FromBody] StartSessionRequest request)
    {
        if (request.VisitorKey == Guid.Empty ||
            request.SessionKey == Guid.Empty)
        {
            return BadRequest();
        }

        var now = DateTime.UtcNow;

        // ---------------------------------------------------------
        // VISITANTE
        // ---------------------------------------------------------

        var visitor = await db.AnalyticsVisitors
            .FirstOrDefaultAsync(x => x.VisitorKey == request.VisitorKey);

        if (visitor == null)
        {
            visitor = new AnalyticsVisitor
            {
                VisitorKey = request.VisitorKey,
                FirstVisitAt = now,
                LastVisitAt = now,
                TotalSessions = 0
            };

            db.AnalyticsVisitors.Add(visitor);

            await db.SaveChangesAsync();
        }

        visitor.LastVisitAt = now;

        // ---------------------------------------------------------
        // SESSÃO
        // ---------------------------------------------------------

        var session = await db.AnalyticsSessions
            .FirstOrDefaultAsync(x => x.SessionKey == request.SessionKey);

        if (session == null)
        {
            session = new AnalyticsSession
            {
                SessionKey = request.SessionKey,
                VisitorId = visitor.Id,

                StartedAt = now,
                LastActivityAt = now,

                LandingPage = Limit(request.PageUrl, 1000),
                Referrer = Limit(request.Referrer, 2000),

                UtmSource = Limit(request.UtmSource, 200),
                UtmMedium = Limit(request.UtmMedium, 200),
                UtmCampaign = Limit(request.UtmCampaign, 300),

                UserAgent = Limit(
                    Request.Headers.UserAgent.ToString(),
                    2000),

                IpHash = GetIpHash()
            };

            db.AnalyticsSessions.Add(session);

            visitor.TotalSessions++;

            await db.SaveChangesAsync();
        }
        else
        {
            session.LastActivityAt = now;

            await db.SaveChangesAsync();
        }

        // ---------------------------------------------------------
        // PAGE VIEW
        // ---------------------------------------------------------

        var pageView = new AnalyticsEvent
        {
            SessionId = session.Id,
            EventType = "page_view",
            EventName = "Page View",
            PageUrl = Limit(request.PageUrl, 2000),
            CreatedAt = now
        };

        db.AnalyticsEvents.Add(pageView);

        await db.SaveChangesAsync();

        return Ok(new
        {
            success = true
        });
    }


    [HttpPost("event")]
    public async Task<IActionResult> TrackEvent(
        [FromBody] TrackEventRequest request)
    {
        if (request.SessionKey == Guid.Empty ||
            string.IsNullOrWhiteSpace(request.EventType))
        {
            return BadRequest();
        }

        var session = await db.AnalyticsSessions
            .FirstOrDefaultAsync(x =>
                x.SessionKey == request.SessionKey);

        if (session == null)
        {
            return NotFound();
        }

        var now = DateTime.UtcNow;

        session.LastActivityAt = now;

        var analyticsEvent = new AnalyticsEvent
        {
            SessionId = session.Id,

            EventType = Limit(
                request.EventType.ToLowerInvariant(),
                100) ?? "unknown",

            EventName = Limit(
                request.EventName,
                300),

            PageUrl = Limit(
                request.PageUrl,
                2000),

            TargetUrl = Limit(
                request.TargetUrl,
                2000),

            CreatedAt = now
        };

        db.AnalyticsEvents.Add(analyticsEvent);

        await db.SaveChangesAsync();

        return Ok(new
        {
            success = true
        });
    }


    // -------------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------------

    private string? GetIpHash()
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrWhiteSpace(ip))
            return null;

        // Salt específico da aplicação.
        // Posteriormente vamos colocar isso em User Secrets.
        var salt = configuration["Analytics:IpHashSalt"]
                   ?? "rhsenso-development";

        var value = $"{salt}:{ip}";

        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(value));

        return Convert.ToHexString(bytes);
    }


    private static string? Limit(
        string? value,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value.Trim();

        return value.Length <= maxLength
            ? value
            : value[..maxLength];
    }
}
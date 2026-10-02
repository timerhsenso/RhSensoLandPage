namespace RhSenso.Web.Models.Analytics.Dto;

public class TrackEventRequest
{
    public Guid SessionKey { get; set; }

    public string EventType { get; set; } = string.Empty;

    public string? EventName { get; set; }

    public string? PageUrl { get; set; }

    public string? TargetUrl { get; set; }
}
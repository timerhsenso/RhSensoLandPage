namespace RhSenso.Web.Models.Analytics.Dto;

public class StartSessionRequest
{
    public Guid VisitorKey { get; set; }

    public Guid SessionKey { get; set; }

    public string? PageUrl { get; set; }

    public string? Referrer { get; set; }

    public string? UtmSource { get; set; }

    public string? UtmMedium { get; set; }

    public string? UtmCampaign { get; set; }
}
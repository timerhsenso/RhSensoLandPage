namespace RhSenso.Web.Models.Analytics;

public class AnalyticsEvent
{
    public long Id { get; set; }

    public long SessionId { get; set; }

    public AnalyticsSession Session { get; set; } = null!;

    // Exemplos:
    // page_view
    // click
    // contact_open
    // contact_submit
    // curriculum_open
    // curriculum_submit
    // whatsapp_click
    public string EventType { get; set; } = string.Empty;

    // Nome amigável do evento.
    // Ex.: "Fale Conosco", "Folha.NET", "WhatsApp"
    public string? EventName { get; set; }

    // Página onde aconteceu.
    public string? PageUrl { get; set; }

    // Destino de um clique, quando houver.
    public string? TargetUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}
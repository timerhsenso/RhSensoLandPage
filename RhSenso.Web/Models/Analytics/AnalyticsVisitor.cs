namespace RhSenso.Web.Models.Analytics;

public class AnalyticsVisitor
{
    public long Id { get; set; }

    // Identificador anônimo gerado pelo navegador.
    // Não identifica nominalmente a pessoa.
    public Guid VisitorKey { get; set; }

    public DateTime FirstVisitAt { get; set; }

    public DateTime LastVisitAt { get; set; }

    public int TotalSessions { get; set; }

    public ICollection<AnalyticsSession> Sessions { get; set; }
        = new List<AnalyticsSession>();
}
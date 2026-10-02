using System.ComponentModel.DataAnnotations;
namespace RhSenso.Web.Models;
public class ContactRequest
{
    public long Id { get; set; }
    [MaxLength(120)] public string Name { get; set; } = string.Empty;
    [MaxLength(180)] public string Email { get; set; } = string.Empty;
    [MaxLength(30)] public string? Phone { get; set; }
    [MaxLength(160)] public string? Company { get; set; }
    [MaxLength(4000)] public string Message { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsHandled { get; set; }
}

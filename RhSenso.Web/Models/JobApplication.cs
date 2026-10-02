using System.ComponentModel.DataAnnotations;
namespace RhSenso.Web.Models;
public class JobApplication
{
    public long Id { get; set; }
    [MaxLength(120)] public string Name { get; set; } = string.Empty;
    [MaxLength(180)] public string Email { get; set; } = string.Empty;
    [MaxLength(30)] public string? Phone { get; set; }
    [MaxLength(120)] public string? Area { get; set; }
    [MaxLength(500)] public string? ResumePath { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

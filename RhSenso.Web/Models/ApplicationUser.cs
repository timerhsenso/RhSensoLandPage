using Microsoft.AspNetCore.Identity;
namespace RhSenso.Web.Models;
public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}

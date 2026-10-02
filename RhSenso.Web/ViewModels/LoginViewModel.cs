using System.ComponentModel.DataAnnotations;
namespace RhSenso.Web.ViewModels;
public class LoginViewModel
{
    [Required, EmailAddress, Display(Name="E-mail")] public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password), Display(Name="Senha")] public string Password { get; set; } = string.Empty;
    [Display(Name="Manter conectado")] public bool RememberMe { get; set; }
}

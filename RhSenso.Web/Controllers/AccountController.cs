using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RhSenso.Web.Models;
using RhSenso.Web.ViewModels;

namespace RhSenso.Web.Controllers;
public class AccountController(SignInManager<ApplicationUser> signInManager) : Controller
{
    [HttpGet("acesso")]
    public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel());

    [HttpPost("acesso")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded) return LocalRedirect(returnUrl ?? "/admin");
        ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
        return View(model);
    }

    [HttpPost("sair")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("acesso-negado")]
    public IActionResult AccessDenied() => View();
}

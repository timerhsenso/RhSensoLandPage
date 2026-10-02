using Microsoft.AspNetCore.Mvc;
namespace RhSenso.Web.Controllers;
public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Error() => View("~/Views/Shared/Error.cshtml");
}

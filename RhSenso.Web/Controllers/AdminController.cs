using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace RhSenso.Web.Controllers;
[Authorize(Roles = "CEO")]
public class AdminController : Controller
{
    [HttpGet("admin")]
    public IActionResult Index() => View();
}

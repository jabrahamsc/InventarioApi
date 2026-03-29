using Microsoft.AspNetCore.Mvc;

namespace InventarioApi.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Productos");
}
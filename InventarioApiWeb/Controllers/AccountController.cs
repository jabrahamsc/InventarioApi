using InventarioApi.Web.Models;
using InventarioApi.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventarioApi.Web.Controllers;

public class AccountController : Controller
{
    private readonly IApiClient _api;

    public AccountController(IApiClient api) => _api = api;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.GetString("JwtToken") is not null)
            return RedirectToAction("Index", "Productos");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var (token, role) = await _api.LoginAsync(model.Username, model.Password);
        if (token is null)
        {
            ModelState.AddModelError(string.Empty,
                "Usuario o contraseña incorrectos.");
            return View(model);
        }

        HttpContext.Session.SetString("JwtToken", token);
        HttpContext.Session.SetString("Username", model.Username);
        HttpContext.Session.SetString("Role", role!);

        return LocalRedirect(returnUrl ?? "/Productos");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
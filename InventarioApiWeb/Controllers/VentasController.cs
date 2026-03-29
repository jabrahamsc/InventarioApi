using InventarioApi.Web.Models;
using InventarioApi.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventarioApi.Web.Controllers;

[Route("[controller]")]
public class VentasController : Controller
{
    private readonly IApiClient _api;

    public VentasController(IApiClient api) => _api = api;

    private string? Token => HttpContext.Session.GetString("JwtToken");

    private IActionResult? RequireLogin()
    {
        if (Token is null)
        {
            TempData["Error"] = "Tu sesion ha expirado. Por favor vuelve a iniciar sesion";
            return RedirectToAction("Login", "Account");
        }
        return null;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        if (RequireLogin() is { } redirect) return redirect;

        var sales = await _api.GetSalesAsync(Token!);
        return View(sales);
    }

    [HttpGet("Registrar")]
    public async Task<IActionResult> Register()
    {
        if (RequireLogin() is { } redirect) return redirect;

        ViewBag.Products = await _api.GetProductsAsync(Token!);
        return View(new RegisterSaleViewModel());
    }

    [HttpPost("Registrar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterSaleViewModel model)
    {
        if (RequireLogin() is { } redirect) return redirect;
        model.Lines = model.Lines
            .Where(l => l.ProductId > 0 && l.Quantity > 0)
            .ToList();

        if (!model.Lines.Any())
        {
            ModelState.AddModelError(string.Empty,
                "Debe agregar al menos un producto a la venta");
            ViewBag.Products = await _api.GetProductsAsync(Token!);
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Products = await _api.GetProductsAsync(Token!);
            return View(model);
        }

        var sale = await _api.CreateSaleAsync(model, Token!);
        if (sale is null)
        {
            ModelState.AddModelError(string.Empty,
                "Error al registrar la venta. Verifique el stock disponible");
            ViewBag.Products = await _api.GetProductsAsync(Token!);
            return View(model);
        }

        TempData["Success"] = $"Venta #{sale.Id} registrada por ${sale.TotalAmount:F2}.";
        return RedirectToAction(nameof(Index));
    }
}
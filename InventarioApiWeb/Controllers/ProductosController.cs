using InventarioApi.Web.Models;
using InventarioApi.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventarioApi.Web.Controllers;

[Route("[controller]")]
public class ProductosController : Controller
{
    private readonly IApiClient _api;

    public ProductosController(IApiClient api) => _api = api;

    private string? Token => HttpContext.Session.GetString("JwtToken");

    private IActionResult? SessionExpired()
    {
        if (Token is null)
        {
            TempData["Error"] = "Tu sesion ha expirado. Por favor vuelve a iniciar sesion";
            return RedirectToAction("Login", "Account");
        }
        return null;
    }

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

        var products = await _api.GetProductsAsync(Token!);
        var lowStock = await _api.GetLowStockAsync(Token!);
        ViewBag.LowStockCount = lowStock.Count;
        return View(products);
    }

    [HttpGet("Crear")]
    public IActionResult Create()
    {
        if (RequireLogin() is { } redirect) return redirect;
        return View(new CreateProductViewModel());
    }

    [HttpPost("Crear")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        if (RequireLogin() is { } redirect) return redirect;
        if (!ModelState.IsValid) return View(model);

        var success = await _api.CreateProductAsync(model, Token!);

        if (SessionExpired() is { } expired) return expired;

        if (!success)
        {
            ModelState.AddModelError(string.Empty,
                "Error al crear el producto, por favor verifique si tiene los permisos necesarios");
            return View(model);
        }

        TempData["Success"] = "Producto creado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Editar/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        if (RequireLogin() is { } redirect) return redirect;

        var product = await _api.GetProductByIdAsync(id, Token!);
        if (product is null) return NotFound();

        var model = new CreateProductViewModel
        {
            Name = product.Name,
            Price = product.Price,
            CurrentStock = product.CurrentStock,
            MinimumStock = product.MinimumStock
        };

        ViewBag.ProductId = id;
        return View(model);
    }

    [HttpPost("Editar/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateProductViewModel model)
    {
        if (RequireLogin() is { } redirect) return redirect;
        if (!ModelState.IsValid)
        {
            ViewBag.ProductId = id;
            return View(model);
        }

        var success = await _api.UpdateProductAsync(id, model, Token!);

        if (SessionExpired() is { } expired) return expired;

        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Error al actualizar el producto, verifique que su usuario sea Administrador");
            ViewBag.ProductId = id;
            return View(model);
        }

        TempData["Success"] = "Producto actualizado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Eliminar/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (RequireLogin() is { } redirect) return redirect;
        if (HttpContext.Session.GetString("Role") != "Admin")
        {
            TempData["Error"] = "No tienes permisos para eliminar productos.";
            return RedirectToAction(nameof(Index));
        }

        await _api.DeleteProductAsync(id, Token!);

        if (SessionExpired() is { } expired) return expired;

        TempData["Success"] = "Producto eliminado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("StockBajo")]
    public async Task<IActionResult> LowStock()
    {
        if (RequireLogin() is { } redirect) return redirect;

        var products = await _api.GetLowStockAsync(Token!);
        return View(products);
    }
}
using InventarioApi.DTOs;
using InventarioApi.Models;
using InventarioApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventarioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService) => _saleService = saleService;

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SaleResponse>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var sales = await _saleService.GetAllAsync();
        return Ok(new ApiResponse<IEnumerable<SaleResponse>>(
            true, "Ventas obtenidas con exjto", sales));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<SaleResponse>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var sale = await _saleService.GetByIdAsync(id);
        return Ok(new ApiResponse<SaleResponse>(
            true, "Venta encontrada", sale));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SaleResponse>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? throw new UnauthorizedAccessException(
                "No se pudo identificar al usuario");

        int userId = int.Parse(userIdClaim);

        var sale = await _saleService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = sale.Id },
            new ApiResponse<SaleResponse>(true, "Venta registrada con exito", sale));
    }
}
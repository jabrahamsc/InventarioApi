using InventarioApi.DTOs;
using InventarioApi.Models;
using InventarioApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventarioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService) =>
        _productService = productService;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponse>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(new ApiResponse<IEnumerable<ProductResponse>>(
            true, "Productos obtenidos", products));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(new ApiResponse<ProductResponse>(
            true, "Producto encontrado", product));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var product = await _productService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = product.Id },
            new ApiResponse<ProductResponse>(true, "Producto creado con exito", product));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        var product = await _productService.UpdateAsync(id, request);
        return Ok(new ApiResponse<ProductResponse>(
            true, "Producto actualizado con exito", product));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LowStockProductResponse>>), 200)]
    public async Task<IActionResult> GetLowStock()
    {
        var products = await _productService.GetLowStockAsync();
        return Ok(new ApiResponse<IEnumerable<LowStockProductResponse>>(
            true, "Productos con stock bajo", products));
    }
}
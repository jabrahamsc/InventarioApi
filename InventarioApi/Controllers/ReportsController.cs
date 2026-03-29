using System;
using System.Security.Claims;
using Azure.Core;
using InventarioApi.DTOs;
using InventarioApi.Models;
using InventarioApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace InventarioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
    private readonly IProductService _productService;

    public ReportsController(IProductService productService) =>
        _productService = productService;

    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LowStockProductResponse>>), 200)]
    public async Task<IActionResult> LowStockReport()
    {
        var products = await _productService.GetLowStockAsync();
        return Ok(new ApiResponse<IEnumerable<LowStockProductResponse>>(
            true,
            $"Reporte generado: {products.Count()} producto(s) requieren reabastecimiento ",
            products));
    }
}
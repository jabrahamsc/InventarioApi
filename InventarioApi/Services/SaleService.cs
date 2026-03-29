using InventarioApi.DTOs;
using InventarioApi.Models;
using InventarioApi.Repositories;

namespace InventarioApi.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepo;
    private readonly IProductRepository _productRepo;

    public SaleService(ISaleRepository saleRepo, IProductRepository productRepo)
    {
        _saleRepo = saleRepo;
        _productRepo = productRepo;
    }

    public async Task<IEnumerable<SaleResponse>> GetAllAsync()
    {
        var sales = await _saleRepo.GetAllAsync();
        return sales.Select(MapToResponse);
    }

    public async Task<SaleResponse> GetByIdAsync(int id)
    {
        var sale = await _saleRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Venta con ID {id} no encontrada");
        return MapToResponse(sale);
    }

    public async Task<SaleResponse> CreateAsync(CreateSaleRequest request, int userId)
    {
        var sale = new Sale { UserId = userId, SaleDate = DateTime.UtcNow };
        decimal total = 0;

        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepo.GetByIdAsync(itemRequest.ProductId)
                ?? throw new KeyNotFoundException(
                    $"Producto con ID {itemRequest.ProductId} no encontrado");

            if (product.CurrentStock < itemRequest.Quantity)
                throw new InvalidOperationException(
                    $"Stock insuficiente para '{product.Name}'" +
                    $"Disponible:{product.CurrentStock}, Solicitado:{itemRequest.Quantity}");

            var saleItem = new SaleItem
            {
                ProductId = product.Id,
                Quantity = itemRequest.Quantity,
                UnitPrice = product.Price
            };
            sale.SaleItems.Add(saleItem);

            product.CurrentStock -= itemRequest.Quantity;
            await _productRepo.UpdateAsync(product);

            total += saleItem.UnitPrice * saleItem.Quantity;
        }

        sale.TotalAmount = total;
        var created = await _saleRepo.CreateAsync(sale);

        var full = await _saleRepo.GetByIdAsync(created.Id)
            ?? throw new Exception("Error al recuperar la venta creada");

        return MapToResponse(full);
    }

    private static SaleResponse MapToResponse(Sale sale) => new(
        sale.Id,
        sale.SaleDate,
        sale.TotalAmount,
        sale.User?.Username ?? "N/A",
        sale.SaleItems.Select(si => new SaleItemResponse(
            si.ProductId,
            si.Product?.Name ?? "N/A",
            si.Quantity,
            si.UnitPrice,
            si.Subtotal)).ToList());
}
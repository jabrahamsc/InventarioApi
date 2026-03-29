using InventarioApi.DTOs;
using InventarioApi.Models;
using InventarioApi.Repositories;

namespace InventarioApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo) => _repo = repo;

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();
        return products.Select(MapToResponse);
    }

    public async Task<ProductResponse> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
        return MapToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            CurrentStock = request.CurrentStock,
            MinimumStock = request.MinimumStock
        };

        var created = await _repo.CreateAsync(product);
        return MapToResponse(created);
    }

    public async Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request)
    {
        var product = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Producto con ID {id} no encontrado");

        product.Name = request.Name;
        product.Price = request.Price;
        product.CurrentStock = request.CurrentStock;
        product.MinimumStock = request.MinimumStock;

        var updated = await _repo.UpdateAsync(product);
        return MapToResponse(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted)
            throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
    }

    public async Task<IEnumerable<LowStockProductResponse>> GetLowStockAsync()
    {
        var products = await _repo.GetLowStockAsync();
        return products.Select(p => new LowStockProductResponse(
            p.Id,
            p.Name,
            p.Price,
            p.CurrentStock,
            p.MinimumStock,
            StockDeficit: p.MinimumStock - p.CurrentStock));
    }
    private static ProductResponse MapToResponse(Product p) =>
        new(p.Id, p.Name, p.Price, p.CurrentStock, p.MinimumStock,
            p.RequiresRestocking, p.CreatedAt);
}
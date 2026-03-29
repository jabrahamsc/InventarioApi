using InventarioApi.DTOs;

namespace InventarioApi.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllAsync();
    Task<ProductResponse> GetByIdAsync(int id);
    Task<ProductResponse> CreateAsync(CreateProductRequest request);
    Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request);
    Task DeleteAsync(int id);
    Task<IEnumerable<LowStockProductResponse>> GetLowStockAsync();
}
using InventarioApi.DTOs;

namespace InventarioApi.Services;

public interface ISaleService
{
    Task<IEnumerable<SaleResponse>> GetAllAsync();
    Task<SaleResponse> GetByIdAsync(int id);
    Task<SaleResponse> CreateAsync(CreateSaleRequest request, int userId);
}
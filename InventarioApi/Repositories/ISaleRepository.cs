using InventarioApi.Models;

namespace InventarioApi.Repositories;

public interface ISaleRepository
{
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<Sale?> GetByIdAsync(int id);
    Task<Sale> CreateAsync(Sale sale);
}
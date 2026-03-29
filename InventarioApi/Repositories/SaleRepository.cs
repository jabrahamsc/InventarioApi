using InventarioApi.Data;
using InventarioApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioApi.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly AppDbContext _context;

    public SaleRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Sale>> GetAllAsync() =>
        await _context.Sales
            .Include(s => s.User)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();

    public async Task<Sale?> GetByIdAsync(int id) =>
        await _context.Sales
            .Include(s => s.User)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Sale> CreateAsync(Sale sale)
    {
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();
        return sale;
    }
}
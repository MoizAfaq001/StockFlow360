using Microsoft.EntityFrameworkCore;
using StockFlow360.Application.Interfaces;
using StockFlow360.Domain.Entities;
using StockFlow360.Infrastructure.Data;

namespace StockFlow360.Infrastructure.Services;

public class PurchaseService : IPurchaseService
{
    private readonly ApplicationDbContext _context;
    private readonly IInventoryService _inventoryService;

    public PurchaseService(ApplicationDbContext context, IInventoryService inventoryService)
    {
        _context = context;
        _inventoryService = inventoryService;
    }

    public async Task<List<Purchase>> GetAllAsync()
    {
        return await _context.Purchases.Include(p => p.Items).ToListAsync();
    }

    public async Task<Purchase?> GetByIdAsync(int id)
    {
        return await _context.Purchases.Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<int> CreatePurchaseAsync(Purchase purchase)
    {
        _context.Purchases.Add(purchase);
        await _context.SaveChangesAsync();

        foreach (var item in purchase.Items)
        {
            await _inventoryService.UpdateStockAsync(item.ProductId, item.Quantity);
        }

        return purchase.Id;
    }
}

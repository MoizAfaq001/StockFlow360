using Microsoft.EntityFrameworkCore;
using StockFlow360.Application.Interfaces;
using StockFlow360.Domain.Entities;
using StockFlow360.Infrastructure.Data;

namespace StockFlow360.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly ApplicationDbContext _context;

    public InventoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Inventory?> GetByProductIdAsync(int productId)
    {
        return await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
    }

    public async Task UpdateStockAsync(int productId, int changeInQuantity)
    {
        var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);

        if (inventory != null)
        {
            inventory.Quantity += changeInQuantity;
        }
        else
        {
            _context.Inventories.Add(new Inventory
            {
                ProductId = productId,
                Quantity = changeInQuantity
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<Inventory>> GetLowStockItemsAsync()
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .Where(i => i.Quantity < i.Product.ReorderLevel)
            .ToListAsync();
    }

    public async Task ProcessReturnAsync(int productId, int returnedQuantity)
    {
        // ReturnedQuantity is added back to inventory
        await UpdateStockAsync(productId, returnedQuantity);
    }
}

using Microsoft.EntityFrameworkCore;
using StockFlow360.Application.DTOs;
using StockFlow360.Application.Interfaces;
using StockFlow360.Domain.Entities;
using StockFlow360.Infrastructure.Data;

namespace StockFlow360.Infrastructure.Services;

public class SaleService : ISaleService
{
    private readonly ApplicationDbContext _context;
    private readonly IInventoryService _inventoryService;

    public SaleService(ApplicationDbContext context, IInventoryService inventoryService)
    {
        _context = context;
        _inventoryService = inventoryService;
    }

    public async Task<List<Sale>> GetAllAsync()
    {
        return await _context.Sales.ToListAsync(); // No need to include Items — Sale.Items is now just DTOs
    }

    public async Task<Sale?> GetByIdAsync(int id)
    {
        return await _context.Sales.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<int> CreateSaleAsync(SaleDTO dto)
    {
        decimal totalAmount = 0;

        var saleDetails = new List<SaleDetail>();

        foreach (var item in dto.Items)
        {
            var taxRate = item.TaxRate;
            var itemSubtotal = item.SellingPrice * item.Quantity;
            item.TaxAmount = itemSubtotal * taxRate;
            item.TotalWithTax = itemSubtotal + item.TaxAmount;

            totalAmount += item.TotalWithTax;

            saleDetails.Add(new SaleDetail
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                SellingPrice = item.SellingPrice                
            });
        }

        var sale = new Sale
        {
            SaleDate = dto.SaleDate,
            TotalAmount = totalAmount,
            Items = saleDetails
        };

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();
        return sale.Id;
    }


    public async Task ProcessReturnAsync(int saleId, List<SaleItemDTO> returnedItems)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == saleId);

        if (sale == null)
            throw new ArgumentException("Sale not found.");

        foreach (var returned in returnedItems)
        {
            var item = sale.Items.FirstOrDefault(i => i.ProductId == returned.ProductId);
            if (item == null) continue;

            item.Quantity -= returned.Quantity;
            if (item.Quantity <= 0)
                _context.Remove(item);

            // Update Inventory
            await _inventoryService.ProcessReturnAsync(returned.ProductId, returned.Quantity);
        }

        sale.TotalAmount = sale.Items.Sum(i => i.SellingPrice * i.Quantity * (1 + i.TaxRate)); // Recalculate total
        await _context.SaveChangesAsync();
    }

}

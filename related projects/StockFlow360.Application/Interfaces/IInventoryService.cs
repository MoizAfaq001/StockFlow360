using StockFlow360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow360.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<Inventory?> GetByProductIdAsync(int productId);
        Task UpdateStockAsync(int productId, int changeInQuantity);
        Task<List<Inventory>> GetLowStockItemsAsync();
        Task ProcessReturnAsync(int productId, int returnedQuantity);
    }
}

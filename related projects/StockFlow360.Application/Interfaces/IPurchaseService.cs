using StockFlow360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow360.Application.Interfaces
{
    public interface IPurchaseService
    {
        Task<List<Purchase>> GetAllAsync();
        Task<Purchase?> GetByIdAsync(int id);
        Task<int> CreatePurchaseAsync(Purchase purchase);
    }
}

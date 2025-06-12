using StockFlow360.Domain.Entities;
using StockFlow360.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow360.Application.Interfaces
{
    public interface ISaleService
    {
        Task<List<Sale>> GetAllAsync();
        Task<Sale?> GetByIdAsync(int id);
        Task<int> CreateSaleAsync(SaleDTO sale);
        Task ProcessReturnAsync(int saleId, List<SaleItemDTO> returnedItems);
    }
}

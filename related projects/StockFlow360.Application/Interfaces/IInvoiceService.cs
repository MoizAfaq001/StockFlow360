using StockFlow360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow360.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<byte[]> GenerateSaleInvoiceAsync(Sale sale);
        Task<byte[]> GeneratePurchaseInvoiceAsync(Purchase purchase);
    }

}

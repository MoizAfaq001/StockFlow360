using StockFlow360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow360.Application.DTOs
{
    public class SaleDTO
    {
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemDTO> Items { get; set; } = []; 
    }

    public class SaleItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal SellingPrice { get; set; }

        public decimal TaxRate { get; set; } = 0.10m;
        public decimal TaxAmount { get; set; }
        public decimal TotalWithTax { get; set; }
        //public Product Product { get; set; } = null!;
    }

}

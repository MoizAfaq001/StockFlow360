using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow360.Application.DTOs
{
    public class PurchaseDTO
    {
        public int SupplierId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public List<PurchaseItemDTO> Items { get; set; } = new();
    }

    public class PurchaseItemDTO
    {
        public int? ProductId { get; set; } // Optional, only needed if existing product
        public ProductDTO? NewProduct { get; set; } 
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
    }

}

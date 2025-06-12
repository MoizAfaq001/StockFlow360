using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow360.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;     // stock keeping unit
        public string Name { get; set; } = string.Empty;
        public decimal SellingPrice { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        public decimal CostPrice { get; set; }
        public int ReorderLevel { get; set; }
    }

}

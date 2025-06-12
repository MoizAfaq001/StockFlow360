using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow360.Application.DTOs
{
    public class InventoryDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsLowStock => Quantity < ReorderLevel;
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow360.Application.Interfaces;
using StockFlow360.Domain.Entities;
using StockFlow360.Application.DTOs;

namespace StockFlow360.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")] // Default restriction on all endpoints
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // GET: api/inventory
        [HttpGet]
        [Authorize] 
        public async Task<IActionResult> GetAll()
        {
            var lowStock = await _inventoryService.GetLowStockItemsAsync();
            return Ok(lowStock);
        }

        // GET: api/inventory/{productId}
        [HttpGet("{productId}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var inventory = await _inventoryService.GetByProductIdAsync(productId);
            if (inventory == null)
                return NotFound("Inventory not found for product.");

            return Ok(inventory);
        }

        // PUT: api/inventory/{productId}
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateStock(int productId, [FromBody] int changeInQuantity)
        {
            await _inventoryService.UpdateStockAsync(productId, changeInQuantity);
            return Ok("Inventory updated.");
        }

        // GET: api/inventory/lowstock
        [HttpGet("lowstock")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetLowStock()
        {
            var items = await _inventoryService.GetLowStockItemsAsync();
            return Ok(items);
        }

        // POST: api/inventory/return
        [HttpPost("return")]
        public async Task<IActionResult> ProcessReturn([FromBody] ReturnDTO dto)
        {
            await _inventoryService.ProcessReturnAsync(dto.ProductId, dto.Quantity);
            return Ok("Return processed and inventory updated.");
        }
    }
}

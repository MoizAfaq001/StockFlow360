using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockFlow360.Application.DTOs;
using StockFlow360.Application.Interfaces;

namespace StockFlow360.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController(ISaleService saleService) : ControllerBase
    {
        private readonly ISaleService _saleService = saleService;

        [HttpGet]
        public async Task<IActionResult> GetAllSales()
        {
            var sales = await _saleService.GetAllAsync();
            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSale(int id)
        {
            var sale = await _saleService.GetByIdAsync(id);
            if (sale is null)
                return NotFound();
            return Ok(sale);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale([FromBody] SaleDTO dto)
        {
            try
            {
                var saleId = await _saleService.CreateSaleAsync(dto);
                //return Ok(new { SaleId = saleId });
                return Redirect($"/api/invoice/sale/{saleId}");
                //return Ok(new
                //{
                //    SaleId = saleId,
                //    InvoiceUrl = $"/api/invoice/sale/{saleId}"
                //});
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }


}

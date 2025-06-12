using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow360.Application.Interfaces;

namespace StockFlow360.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ISaleService _saleService;
        private readonly IPurchaseService _purchaseService;

        public InvoiceController(IInvoiceService invoiceService, ISaleService saleService, IPurchaseService purchaseService)
        {
            _invoiceService = invoiceService;
            _saleService = saleService;
            _purchaseService = purchaseService;
        }

        [HttpGet("sale/{id}")]
        public async Task<IActionResult> GetSaleInvoice(int id)
        {
            var sale = await _saleService.GetByIdAsync(id);
            if (sale == null) return NotFound();

            var pdf = await _invoiceService.GenerateSaleInvoiceAsync(sale);
            return File(pdf, "application/pdf", $"SaleInvoice_{id}.pdf");
        }

        [HttpGet("purchase/{id}")]
        public async Task<IActionResult> GetPurchaseInvoice(int id)
        {
            var purchase = await _purchaseService.GetByIdAsync(id);
            if (purchase == null) return NotFound();

            var pdf = await _invoiceService.GeneratePurchaseInvoiceAsync(purchase);
            return File(pdf, "application/pdf", $"PurchaseInvoice_{id}.pdf");
        }
    }

}

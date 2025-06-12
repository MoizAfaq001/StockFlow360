using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow360.Application.DTOs;
using StockFlow360.Application.Interfaces;
using StockFlow360.Domain.Entities;

namespace StockFlow360.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IProductService _productService;

        public PurchaseController(IPurchaseService purchaseService, IProductService productService)
        {
            _purchaseService = purchaseService;
            _productService = productService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Supplier")]
        public async Task<IActionResult> Create(PurchaseDTO dto)
        {
            var purchase = new Purchase
            {
                SupplierId = dto.SupplierId,
                PurchaseDate = dto.PurchaseDate,
                Items = new List<PurchaseDetail>()
            };

            foreach (var item in dto.Items)
            {
                int productId;

                // If a new product is provided by the supplier
                if (item.ProductId == null && item.NewProduct != null)
                {
                    var newProduct = new Product
                    {
                        SKU = item.NewProduct.SKU,
                        Name = item.NewProduct.Name,
                        SellingPrice = item.NewProduct.SellingPrice,
                        CostPrice = item.NewProduct.CostPrice,
                        ReorderLevel = item.NewProduct.ReorderLevel,
                        SupplierId = dto.SupplierId
                    };

                    await _productService.AddAsync(newProduct);
                    productId = newProduct.Id;
                }
                else if (item.ProductId.HasValue)
                {
                    productId = item.ProductId.Value;
                }
                else
                {
                    return BadRequest("Each item must have either ProductId or NewProduct specified.");
                }

                purchase.Items.Add(new PurchaseDetail
                {
                    ProductId = productId,
                    Quantity = item.Quantity,
                    CostPrice = item.CostPrice
                });
            }

            var purchaseId = await _purchaseService.CreatePurchaseAsync(purchase);
            //return Ok("Purchase recorded successfully.");
            return Redirect($"/api/invoice/purchase/{purchaseId}");
            //return Ok(new
            //{
            //    PurchaseId = purchaseId,
            //    InvoiceUrl = $"/api/invoice/purchase/{purchaseId}"
            //});


        }
    }
}

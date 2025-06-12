using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow360.Domain.Entities;
using StockFlow360.Application.Interfaces;

namespace StockFlow360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/product
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Cashier,Supplier")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Cashier,Supplier")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }

        // POST: api/product
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            await _productService.AddAsync(product);
            return Ok("Product created successfully");
        }

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] Product updatedProduct)
        {
            var existing = await _productService.GetProductByIdAsync(id);
            if (existing == null)
                return NotFound("Product not found");

            updatedProduct.Id = id;
            await _productService.UpdateAsync(updatedProduct);
            return Ok("Product updated successfully");
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _productService.GetProductByIdAsync(id);
            if (existing == null)
                return NotFound("Product not found");

            await _productService.DeleteAsync(id);
            return Ok("Product deleted successfully");
        }
    }
}

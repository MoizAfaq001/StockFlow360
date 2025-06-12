using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow360.Application.Interfaces;
using StockFlow360.Domain.Entities;

namespace StockFlow360.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Supplier")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    // GET: api/supplier
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _supplierService.GetAllAsync();
        return Ok(suppliers);
    }

    // GET: api/supplier/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        if (supplier == null)
            return NotFound();

        return Ok(supplier);
    }

    // POST: api/supplier
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Supplier supplier)
    {
        await _supplierService.AddAsync(supplier);
        return Ok();
    }

    // PUT: api/supplier/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Supplier supplier)
    {
        if (id != supplier.Id)
            return BadRequest("Supplier ID mismatch.");

        await _supplierService.UpdateAsync(supplier);
        return Ok();
    }

    // DELETE: api/supplier/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _supplierService.DeleteAsync(id);
        return Ok();
    }
}

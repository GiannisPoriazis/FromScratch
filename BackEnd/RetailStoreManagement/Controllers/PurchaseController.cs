using Microsoft.AspNetCore.Mvc;
using RetailStoreManagement.Models;
using RetailStoreManagement.Interfaces;

namespace RetailStoreManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _service;

        public PurchaseController(IPurchaseService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseDto>> GetPurchases(int id)
        {
            var purchase = await _service.GetByIdAsync(id);
            return purchase == null ? NotFound() : purchase;
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseDto>> CreatePurchase(PurchaseCreateDto createDto)
        {
            var (created, error, missing) = await _service.CreateAsync(createDto);

            if (error != null)
            {
                if (error.StartsWith("Customer "))
                    return NotFound(error);

                if (missing != null && missing.Any())
                    return BadRequest(new { message = error, missing });

                return BadRequest(error);
            }

            return CreatedAtAction(nameof(GetPurchases), new { id = created!.Id }, created);
        }
    }
}

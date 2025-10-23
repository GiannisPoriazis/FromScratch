using Microsoft.AspNetCore.Mvc;
using RetailStoreManagement.Models;
using RetailStoreManagement.Interfaces;

namespace RetailStoreManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet("{sku}")]
        public async Task<ActionResult<Product>> GetProduct(string sku)
        {
            var product = await _service.GetBySkuAsync(sku);
            return product == null ? NotFound() : product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            var created = await _service.CreateAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { sku = created.SKU }, created);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateProduct(Product product)
        {
            await _service.UpdateAsync(product);
            return NoContent();
        }

        [HttpDelete("{sku}")]
        public async Task<ActionResult> DeleteProduct(string sku)
        {
            var result = await _service.DeleteAsync(sku);
            return result switch
            {
                DeleteResult.NotFound => NotFound(),
                DeleteResult.Deleted => NoContent(),
                _ => StatusCode(500)
            };
        }
    }
}

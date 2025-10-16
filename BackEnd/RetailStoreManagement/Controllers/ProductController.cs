using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailStoreManagement.Models;

namespace RetailStoreManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly RetailStoreContext _context;

        public ProductController(RetailStoreContext context)
        {
            _context = context;
        }

        [HttpGet("{sku}")]
        public async Task<ActionResult<Product>> GetProduct(string sku)
        {
            var product = await _context.Products.FindAsync(sku);
            return product == null ? NotFound() : product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            string sku;

            do
            {
                sku = GenerateSKU();
            } 
            while (await _context.Products.AnyAsync(p => p.SKU == sku));

            product.SKU = sku;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { sku = product.SKU }, product);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateProduct(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{sku}")]
        public async Task<ActionResult> DeleteProduct(string sku)
        {
            var product = await _context.Products.FindAsync(sku);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private string GenerateSKU()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }
    }
}

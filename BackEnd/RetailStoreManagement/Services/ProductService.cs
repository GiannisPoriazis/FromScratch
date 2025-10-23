using Microsoft.EntityFrameworkCore;
using RetailStoreManagement.Models;
using RetailStoreManagement.Interfaces;

namespace RetailStoreManagement.Services
{
    public class ProductService : IProductService
    {
        private readonly RetailStoreContext _context;

        public ProductService(RetailStoreContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetBySkuAsync(string sku)
        {
            return await _context.Products.FindAsync(sku);
        }

        public async Task<Product> CreateAsync(Product product)
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
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<DeleteResult> DeleteAsync(string sku)
        {
            var product = await _context.Products.FindAsync(sku);
            if (product == null) return DeleteResult.NotFound;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return DeleteResult.Deleted;
        }

        private string GenerateSKU()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }
    }
}

using RetailStoreManagement.Models;

namespace RetailStoreManagement.Interfaces
{
    public interface IProductService
    {
        Task<Product?> GetBySkuAsync(string sku);
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task<DeleteResult> DeleteAsync(string sku);
    }
}

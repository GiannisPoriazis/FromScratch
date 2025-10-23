using RetailStoreManagement.Models;

namespace RetailStoreManagement.Interfaces
{
    public enum DeleteResult
    {
        Deleted,
        NotFound,
        HasPurchases
    }

    public interface ICustomerService
    {
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer> CreateAsync(CustomerDto dto);
        Task UpdateAsync(Customer customer);
        Task<DeleteResult> DeleteAsync(int id);
    }
}

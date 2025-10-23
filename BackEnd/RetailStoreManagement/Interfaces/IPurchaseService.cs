using RetailStoreManagement.Models;

namespace RetailStoreManagement.Interfaces
{
    public interface IPurchaseService
    {
        Task<PurchaseDto?> GetByIdAsync(int id);
        Task<(PurchaseDto? created, string? error, List<string>? missingProductIds)> CreateAsync(PurchaseCreateDto createDto);
    }
}

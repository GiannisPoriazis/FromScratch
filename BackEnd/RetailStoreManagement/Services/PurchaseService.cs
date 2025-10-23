using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RetailStoreManagement.Models;
using RetailStoreManagement.Interfaces;

namespace RetailStoreManagement.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly RetailStoreContext _context;
        private readonly IMapper _mapper;

        public PurchaseService(RetailStoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PurchaseDto?> GetByIdAsync(int id)
        {
            return await _context.Purchases
                .AsNoTracking()
                .Where(p => p.Id == id)
                .ProjectTo<PurchaseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<(PurchaseDto? created, string? error, List<string>? missingProductIds)> CreateAsync(PurchaseCreateDto createDto)
        {
            if (createDto == null)
            {
                return (null, "Invalid payload", null);
            }

            if (createDto.PurchaseProducts.Any(pp => pp.Quantity <= 0))
            {
                return (null, "Quantity must be at least 1 for purchase products.", null);
            }

            var customerExists = await _context.Customers.AnyAsync(c => c.Id == createDto.CustomerId);
            if (!customerExists)
            {
                return (null, $"Customer {createDto.CustomerId} not found.", null);
            }

            var productIds = createDto.PurchaseProducts.Select(x => x.ProductId).Distinct().ToList();

            var existing = await _context.Products
                .Where(p => productIds.Contains(p.SKU))
                .Select(p => p.SKU)
                .ToListAsync();

            var missing = productIds.Except(existing).ToList();
            if (missing.Any())
            {
                return (null, "Some of the products were not found.", missing);
            }

            var purchase = _mapper.Map<Purchase>(createDto);
            if (purchase == null)
            {
                return (null, "Mapping failed", null);
            }

            purchase.PurchaseDate = DateTime.UtcNow;

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            var created = await _context.Purchases
                .AsNoTracking()
                .Where(p => p.Id == purchase.Id)
                .ProjectTo<PurchaseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return (created, null, null);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailStoreManagement.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace RetailStoreManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly RetailStoreContext _context;
        private readonly IMapper _mapper;

        public PurchaseController(RetailStoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseDto>> GetPurchases(int id)
        {
            var purchase = await _context.Purchases
                .AsNoTracking()
                .Where(p => p.Id == id)
                .ProjectTo<PurchaseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (purchase == null)
            {
                return NotFound();
            }

            return purchase;
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseDto>> CreatePurchase(PurchaseCreateDto createDto)
        {
            if (createDto == null)
            {
                return BadRequest();
            }

            if (createDto.PurchaseProducts.Any(pp => pp.Quantity <= 0))
            {
                return BadRequest("Quantity must be at least 1 for purchase products.");
            }

            var customerExists = await _context.Customers.AnyAsync(c => c.Id == createDto.CustomerId);

            if (!customerExists)
            {
                return NotFound($"Customer {createDto.CustomerId} not found.");
            }

            var productIds = createDto.PurchaseProducts.Select(x => x.ProductId).Distinct().ToList();

            var existing = await _context.Products
                .Where(p => productIds.Contains(p.SKU))
                .Select(p => p.SKU)
                .ToListAsync();

            var missing = productIds.Except(existing).ToList();

            if (missing.Any())
            {
                return BadRequest(new { message = "Some of the products were not found.", missing });
            }

            var purchase = _mapper.Map<Purchase>(createDto);

            if(purchase == null)
            {
                return BadRequest();
            }

            purchase.PurchaseDate = DateTime.UtcNow;

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            var created = await _context.Purchases
                .AsNoTracking()
                .Where(p => p.Id == purchase.Id)
                .ProjectTo<PurchaseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetPurchases), new { id = created!.Id }, created);
        }
    }
}

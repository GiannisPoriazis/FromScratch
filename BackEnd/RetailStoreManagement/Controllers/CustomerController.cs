using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailStoreManagement.Models;

namespace RetailStoreManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly RetailStoreContext _context;
        private readonly IMapper _mapper;

        public CustomerController(RetailStoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            return customer == null ? NotFound() : customer;
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(CustomerDto customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);

            if (customer == null)
            {
                return BadRequest();
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCustomer(Customer customer)
        {
            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            
            if(customer == null)
            {
                return NotFound();
            }

            var hasPurchases = await _context.Purchases.AnyAsync(p => p.CustomerId == id);
            if (hasPurchases) return Conflict("Customer cannot be deleted. Purchases related to the customer were found.");

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

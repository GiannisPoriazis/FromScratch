using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RetailStoreManagement.Models;
using RetailStoreManagement.Interfaces;

namespace RetailStoreManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;
        private readonly IMapper _mapper;

        public CustomerController(ICustomerService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _service.GetByIdAsync(id);
            return customer == null ? NotFound() : customer;
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(CustomerDto customerDto)
        {
            var customer = await _service.CreateAsync(customerDto);

            if (customer == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCustomer(Customer customer)
        {
            await _service.UpdateAsync(customer);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var result = await _service.DeleteAsync(id);

            return result switch
            {
                DeleteResult.NotFound => NotFound(),
                DeleteResult.HasPurchases => Conflict("Customer cannot be deleted. Purchases related to the customer were found."),
                DeleteResult.Deleted => NoContent(),
                _ => StatusCode(500)
            };
        }
    }
}

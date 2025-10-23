using Microsoft.EntityFrameworkCore;
using RetailStoreManagement.Models;
using RetailStoreManagement.Interfaces;
using System;

namespace RetailStoreManagement.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly RetailStoreContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public CustomerService(RetailStoreContext context, AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customer> CreateAsync(CustomerDto dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<DeleteResult> DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return DeleteResult.NotFound;

            _context.Customers.Remove(customer);
            try
            {
                await _context.SaveChangesAsync();
                return DeleteResult.Deleted;
            }
            catch (DbUpdateException ex)
            {
                return DeleteResult.HasPurchases;
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ToDoApi.Models;
using ToDoApi.Data;

namespace ToDoApi.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ToDoContext _context;

        public CustomerRepository(ToDoContext context)
        {
            _context = context;
        }

        // Il ritorno del metodo deve essere coerente con l'interfaccia ICustomerRepository
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            // Rimuovi la nullabilità nel ritorno per evitare conflitti con l'interfaccia
            return await _context.Set<Customer>().ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            // Il ritorno di FindAsync() potrebbe essere null, quindi è giusto usare Customer?
            return await _context.Set<Customer>().FindAsync(id);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            _context.Set<Customer>().Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _context.Set<Customer>().Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _context.Set<Customer>().FindAsync(id);
            if (customer != null)
            {
                _context.Set<Customer>().Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
    }
}

using RetailStoreManagement.Models;

namespace RetailStoreManagement.Tests
{
    public class Seeding
    {
        public static void InitializeTestDB(RetailStoreContext db)
        {
            db.Products.AddRange(GetProducts());
            db.Customers.AddRange(GetCustomers());
            db.SaveChanges();
        }

        private static List<Product> GetProducts()
        {
            return new List<Product>()
            {
                new Product() { SKU = "1234567A", Title = "Wireless Mouse", Price = 5.50M },
                new Product() { SKU = "1234567B", Title = "Wireless Keyboard", Price = 105.50M },
                new Product() { SKU = "1234567C", Title = "Gaming Chair", Price = 75.80M },
            };
        }

        private static List<Customer> GetCustomers()
        {
            return new List<Customer>()
            {
                new Customer() { Id = 1, FullName = "Giannis Poriazis", Email = "giannisporiazis@gmail.com" },
                new Customer() { Id = 2, FullName = "Roza Boukouvala", Email = "rozaboukouvala@gmail.com" },
                new Customer() { Id = 3, FullName = "Stefanos Poriazis", Email = "test@mail.com" },
            };
        }
    }
}

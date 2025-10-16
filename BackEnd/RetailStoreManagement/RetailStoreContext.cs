using Microsoft.EntityFrameworkCore;
using RetailStoreManagement.Models;

namespace RetailStoreManagement
{
    public class RetailStoreContext : DbContext
    {
        public RetailStoreContext(DbContextOptions<RetailStoreContext> options) : base(options)
        {

        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Purchase> Purchases => Set<Purchase>();
        public DbSet<PurchaseProduct> PurchaseProducts => Set<PurchaseProduct>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PurchaseProduct>()
                .HasKey(pp => new { pp.PurchaseId, pp.ProductId });

            modelBuilder.Entity<PurchaseProduct>()
                .HasOne(pp => pp.Purchase)
                .WithMany(p => p.PurchaseProducts)
                .HasForeignKey(pp => pp.PurchaseId);

            modelBuilder.Entity<PurchaseProduct>()
                .HasOne(pp => pp.Product)
                .WithMany()
                .HasForeignKey(pp => pp.ProductId);
        }
    }
}

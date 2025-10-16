using System.ComponentModel.DataAnnotations;

namespace RetailStoreManagement.Models
{
    public class PurchaseProduct
    {
        public int PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }
        public string ProductId { get; set; } = null!;
        public Product? Product { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class PurchaseProductDto 
    {
        public int PurchaseId { get; set; }
        public string ProductId { get; set; } = null!; 
        public int Quantity { get; set; } 
    }

    public class PurchaseProductCreateDto
    {
        public string ProductId { get; set; } = null!;
        public int Quantity { get; set; }
    }
}

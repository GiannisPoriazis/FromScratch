using System.ComponentModel.DataAnnotations;

namespace RetailStoreManagement.Models
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int CustomerId { get; set; }

        public ICollection<PurchaseProduct> PurchaseProducts { get; set; } = new List<PurchaseProduct>();
    }

    public class PurchaseDto
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int CustomerId { get; set; }
        public List<PurchaseProductDto> PurchaseProducts { get; set; } = new();
    }

    public class PurchaseCreateDto
    {
        public int CustomerId { get; set; }
        public List<PurchaseProductCreateDto> PurchaseProducts { get; set; } = new();
    }
}

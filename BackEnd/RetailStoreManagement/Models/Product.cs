using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RetailStoreManagement.Models
{
    public class Product
    {
        [Key]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public string Title { get; set; } = null!;

        [Precision(18, 2)]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}

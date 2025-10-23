using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RetailStoreManagement.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        public string FullName { get; set; } = null!;

        [EmailAddress]
        public string? Email { get; set; }

        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }

    public class CustomerDto
    {
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
    }
}

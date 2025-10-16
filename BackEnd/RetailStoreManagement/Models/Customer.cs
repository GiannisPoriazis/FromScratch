using System.ComponentModel.DataAnnotations;

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
    }

    public class CustomerDto
    {
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Task2.Core.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        [MaxLength(100)]
        public string? Category { get; set; }
        
        [MaxLength(50)]
        public string? Brand { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
} 
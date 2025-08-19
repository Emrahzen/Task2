using System.ComponentModel.DataAnnotations;

namespace Task2.Core.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        public string? PhoneNumber { get; set; }
        
        public string FullName => $"{FirstName} {LastName}";
    }
} 
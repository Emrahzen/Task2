using Task2.Core.Entities;

namespace Task2.Core.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
        string? GetUserIdFromToken(string token);
    }
} 
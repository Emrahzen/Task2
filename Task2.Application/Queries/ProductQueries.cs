using MediatR;
using Task2.Application.DTOs;

namespace Task2.Application.Queries
{
    public class GetProductByIdQuery : IRequest<ProductResponseDto?>
    {
        public int Id { get; set; }
    }

    public class GetAllProductsQuery : IRequest<IEnumerable<ProductResponseDto>>
    {
        public ProductFilterDto? Filter { get; set; }
    }

    public class GetProductsByCategoryQuery : IRequest<IEnumerable<ProductResponseDto>>
    {
        public string Category { get; set; } = string.Empty;
    }

    public class SearchProductsQuery : IRequest<IEnumerable<ProductResponseDto>>
    {
        public string SearchTerm { get; set; } = string.Empty;
    }
} 
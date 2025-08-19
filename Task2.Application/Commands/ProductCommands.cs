using MediatR;
using Task2.Application.DTOs;

namespace Task2.Application.Commands
{
    public class CreateProductCommand : IRequest<ProductResponseDto>
    {
        public CreateProductDto CreateProductDto { get; set; } = new();
    }

    public class UpdateProductCommand : IRequest<ProductResponseDto>
    {
        public int Id { get; set; }
        public UpdateProductDto UpdateProductDto { get; set; } = new();
    }

    public class DeleteProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
} 
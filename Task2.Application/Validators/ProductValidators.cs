using FluentValidation;
using Task2.Application.Commands;

namespace Task2.Application.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.CreateProductDto.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

            RuleFor(x => x.CreateProductDto.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");

            RuleFor(x => x.CreateProductDto.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be greater than or equal to 0");

            RuleFor(x => x.CreateProductDto.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.CreateProductDto.Category)
                .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");

            RuleFor(x => x.CreateProductDto.Brand)
                .MaximumLength(50).WithMessage("Brand cannot exceed 50 characters");
        }
    }

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0");

            RuleFor(x => x.UpdateProductDto.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

            RuleFor(x => x.UpdateProductDto.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");

            RuleFor(x => x.UpdateProductDto.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be greater than or equal to 0");
        }
    }
} 
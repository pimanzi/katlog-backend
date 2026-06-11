using FluentValidation;
using katlog_backend.DTOs;
using katlog_backend.Enums;

namespace katlog_backend.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(200)
            .WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.ProductCode)
            .NotEmpty()
            .WithMessage("Product code is required")
            .MaximumLength(50)
            .WithMessage("Product code cannot exceed 50 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid product status");

        RuleFor(x => x.Season)
            .IsInEnum()
            .WithMessage("Invalid season");

        RuleFor(x => x.TargetMarket)
            .NotEmpty()
            .WithMessage("At least one target market is required")
            .Must(tm => tm.All(t => new[]
            {
                "Men", "Women", "Boys",
                "Girls", "Unisex", "Adults", "All"
            }.Contains(t)))
            .WithMessage("Invalid target market value");

        RuleFor(x => x.BrandId)
            .GreaterThan(0)
            .WithMessage("Valid brand is required");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Valid category is required");
    }
}

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => x.Name is not null);

        RuleFor(x => x.ProductCode)
            .MaximumLength(50)
            .When(x => x.ProductCode is not null);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);

        RuleFor(x => x.Season)
            .IsInEnum()
            .When(x => x.Season.HasValue);

        RuleFor(x => x.TargetMarket)
            .Must(tm => tm!.All(t => new[]
            {
                "Men", "Women", "Boys",
                "Girls", "Unisex", "Adults", "All"
            }.Contains(t)))
            .WithMessage("Invalid target market value")
            .When(x => x.TargetMarket is not null);

        RuleFor(x => x.BrandId)
            .GreaterThan(0)
            .When(x => x.BrandId.HasValue);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue);
    }
}
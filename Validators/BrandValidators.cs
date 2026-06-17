using FluentValidation;
using katlog_backend.DTOs;

namespace katlog_backend.Validators;

public class CreateBrandValidator : AbstractValidator<CreateBrandDto>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Brand name is required")
            .MaximumLength(100)
            .WithMessage("Brand name cannot exceed 100 characters");
    }
}

public class UpdateBrandValidator : AbstractValidator<UpdateBrandDto>
{
    public UpdateBrandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("Brand name cannot exceed 100 characters")
            .When(x => x.Name is not null);
    }
}
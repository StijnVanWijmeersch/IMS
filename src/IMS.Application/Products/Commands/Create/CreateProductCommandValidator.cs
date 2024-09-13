using FluentValidation;
using IMS.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using IMS.Domain.Products;

namespace IMS.Application.Products.Commands.Create;

// This is the validation schema for the CreateProductCommand
internal sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(IIMSDbContext context)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("De naam van een product kan niet leeg zijn");

        RuleFor(p => p.Sku)
            .NotEmpty()
            .WithMessage("Het artikelnummer is een verplicht veld");

        RuleFor(p => p.Sku)
            .MustAsync(async (sku, _) =>
            {
                var newSku = new Sku(sku);
                var exists = await context.Products.AnyAsync(p => p.Sku == newSku, _);

                return !exists;
            })
            .WithMessage("Het artikelnummer bestaat al");

        RuleFor(p => p.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Het aantal producten kan niet minder dan 0 zijn");

        RuleFor(p => p.InStock)
            .Equal(true)
            .When(p => p.StockQuantity > 0)
            .WithMessage("Het product is niet op voorraad");

        RuleFor(p => p.InStock)
            .Equal(false)
            .When(p => p.StockQuantity == 0)
            .WithMessage("Het product is op voorraad");

        RuleFor(p => p.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("De prijs kan niet minder dan 0 zijn");
    }
}

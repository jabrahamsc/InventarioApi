using FluentValidation;
using InventarioApi.DTOs;

namespace InventarioApi.Validators;

public class CreateSaleValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("La venta debe tener al menos un producto");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .GreaterThan(0).WithMessage("El ID del producto debe ser valido");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0");
        });
    }
}
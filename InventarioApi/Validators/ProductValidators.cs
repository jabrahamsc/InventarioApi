using FluentValidation;
using InventarioApi.DTOs;

namespace InventarioApi.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del producto es requerido")
            .MaximumLength(150).WithMessage("El nombre no puede superar 150 caracteres");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

        RuleFor(x => x.CurrentStock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock actual no puede ser negativo");

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock minimo no puede ser negativo");
    }
}

public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del producto es requerid")
            .MaximumLength(150).WithMessage("El nombre no puede superar 150 caracteres");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

        RuleFor(x => x.CurrentStock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock actual no puede ser negativo");

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo");
    }
}
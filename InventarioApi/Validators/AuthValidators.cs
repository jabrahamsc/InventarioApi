using Azure.Core;
using FluentValidation;
using InventarioApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InventarioApi.Validators;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El usuario no puede estar vacio");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña no puede estar vacia");
    }
}

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El usuario no puede estar vacio")
            .MaximumLength(25).WithMessage("El usuario no puede superar 25 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email non puede estar vacio")
            .EmailAddress().WithMessage("El email debe tener un formato valico");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña no puede estar vacia")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("El rol no puede ser vacio")
            .Must(r => r == "Admin" || r == "Employee")
            .WithMessage("El rol debe ser 'Admin' o 'Employee'");
    }
}
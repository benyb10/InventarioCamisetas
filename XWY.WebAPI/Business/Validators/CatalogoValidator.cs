using FluentValidation;
using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Validators
{
    public class RolCreateValidator : AbstractValidator<RolCreateDto>
    {
        public RolCreateValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del rol es obligatorio")
                .MaximumLength(50).WithMessage("El nombre del rol no puede exceder 50 caracteres")
                .Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$").WithMessage("El nombre del rol solo puede contener letras y espacios");

            RuleFor(x => x.Descripcion)
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));
        }
    }

    public class RolUpdateValidator : AbstractValidator<RolUpdateDto>
    {
        public RolUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del rol es obligatorio")
                .MaximumLength(50).WithMessage("El nombre del rol no puede exceder 50 caracteres")
                .Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$").WithMessage("El nombre del rol solo puede contener letras y espacios");

            RuleFor(x => x.Descripcion)
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));
        }
    }

    public class CategoriaCreateValidator : AbstractValidator<CategoriaCreateDto>
    {
        public CategoriaCreateValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la categoría es obligatorio")
                .MaximumLength(100).WithMessage("El nombre de la categoría no puede exceder 100 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(300).WithMessage("La descripción no puede exceder 300 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));
        }
    }

    public class CategoriaUpdateValidator : AbstractValidator<CategoriaUpdateDto>
    {
        public CategoriaUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la categoría es obligatorio")
                .MaximumLength(100).WithMessage("El nombre de la categoría no puede exceder 100 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(300).WithMessage("La descripción no puede exceder 300 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));
        }
    }

    public class EstadoArticuloCreateValidator : AbstractValidator<EstadoArticuloCreateDto>
    {
        public EstadoArticuloCreateValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del estado es obligatorio")
                .MaximumLength(50).WithMessage("El nombre del estado no puede exceder 50 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));
        }
    }

    public class EstadoArticuloUpdateValidator : AbstractValidator<EstadoArticuloUpdateDto>
    {
        public EstadoArticuloUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del estado es obligatorio")
                .MaximumLength(50).WithMessage("El nombre del estado no puede exceder 50 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));
        }
    }

    public class EstadoPrestamoCreateValidator : AbstractValidator<EstadoPrestamoCreateDto>
    {
        public EstadoPrestamoCreateValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del estado es obligatorio")
                .MaximumLength(50).WithMessage("El nombre del estado no puede exceder 50 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));
        }
    }

    public class EstadoPrestamoUpdateValidator : AbstractValidator<EstadoPrestamoUpdateDto>
    {
        public EstadoPrestamoUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del estado es obligatorio")
                .MaximumLength(50).WithMessage("El nombre del estado no puede exceder 50 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));
        }

    }
    }

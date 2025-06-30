using FluentValidation;
using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Validators
{
    public class UsuarioCreateValidator : AbstractValidator<UsuarioCreateDto>
    {
        public UsuarioCreateValidator()
        {
            RuleFor(x => x.Cedula)
                .NotEmpty().WithMessage("La cédula es obligatoria")
                .Length(10).WithMessage("La cédula debe tener exactamente 10 dígitos")
                .Matches("^[0-9]+$").WithMessage("La cédula solo debe contener números")
                .Must(BeValidEcuadorianCedula).WithMessage("La cédula no es válida");

            RuleFor(x => x.Nombres)
                .NotEmpty().WithMessage("Los nombres son obligatorios")
                .MaximumLength(100).WithMessage("Los nombres no pueden exceder 100 caracteres")
                .Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$").WithMessage("Los nombres solo pueden contener letras y espacios");

            RuleFor(x => x.Apellidos)
                .NotEmpty().WithMessage("Los apellidos son obligatorios")
                .MaximumLength(100).WithMessage("Los apellidos no pueden exceder 100 caracteres")
                .Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$").WithMessage("Los apellidos solo pueden contener letras y espacios");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("El formato del email no es válido")
                .MaximumLength(150).WithMessage("El email no puede exceder 150 caracteres");

            RuleFor(x => x.Telefono)
                .MaximumLength(15).WithMessage("El teléfono no puede exceder 15 caracteres")
                .Matches("^[0-9+\\-\\s]*$").WithMessage("El teléfono solo puede contener números, espacios, + y -")
                .When(x => !string.IsNullOrEmpty(x.Telefono));

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres")
                .MaximumLength(50).WithMessage("La contraseña no puede exceder 50 caracteres");

            RuleFor(x => x.RolId)
                .GreaterThan(0).WithMessage("Debe seleccionar un rol válido");
        }

        private bool BeValidEcuadorianCedula(string cedula)
        {
            if (string.IsNullOrEmpty(cedula) || cedula.Length != 10)
                return false;

            if (!cedula.All(char.IsDigit))
                return false;

            var digits = cedula.Select(c => int.Parse(c.ToString())).ToArray();
            var province = int.Parse(cedula.Substring(0, 2));

            if (province < 1 || province > 24)
                return false;

            var sum = 0;
            for (int i = 0; i < 9; i++)
            {
                var digit = digits[i];
                if (i % 2 == 0)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }
                sum += digit;
            }

            var checkDigit = (10 - (sum % 10)) % 10;
            return checkDigit == digits[9];
        }
    }

    public class UsuarioUpdateValidator : AbstractValidator<UsuarioUpdateDto>
    {
        public UsuarioUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

            RuleFor(x => x.Cedula)
                .NotEmpty().WithMessage("La cédula es obligatoria")
                .Length(10).WithMessage("La cédula debe tener exactamente 10 dígitos")
                .Matches("^[0-9]+$").WithMessage("La cédula solo debe contener números");

            RuleFor(x => x.Nombres)
                .NotEmpty().WithMessage("Los nombres son obligatorios")
                .MaximumLength(100).WithMessage("Los nombres no pueden exceder 100 caracteres")
                .Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$").WithMessage("Los nombres solo pueden contener letras y espacios");

            RuleFor(x => x.Apellidos)
                .NotEmpty().WithMessage("Los apellidos son obligatorios")
                .MaximumLength(100).WithMessage("Los apellidos no pueden exceder 100 caracteres")
                .Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$").WithMessage("Los apellidos solo pueden contener letras y espacios");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("El formato del email no es válido")
                .MaximumLength(150).WithMessage("El email no puede exceder 150 caracteres");

            RuleFor(x => x.Telefono)
                .MaximumLength(15).WithMessage("El teléfono no puede exceder 15 caracteres")
                .Matches("^[0-9+\\-\\s]*$").WithMessage("El teléfono solo puede contener números, espacios, + y -")
                .When(x => !string.IsNullOrEmpty(x.Telefono));

            RuleFor(x => x.RolId)
                .GreaterThan(0).WithMessage("Debe seleccionar un rol válido");
        }
    }

    public class UsuarioLoginValidator : AbstractValidator<UsuarioLoginDto>
    {
        public UsuarioLoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("El formato del email no es válido");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria");
        }
    }

    public class UsuarioPasswordChangeValidator : AbstractValidator<UsuarioPasswordChangeDto>
    {
        public UsuarioPasswordChangeValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("La contraseña actual es obligatoria");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La nueva contraseña es obligatoria")
                .MinimumLength(6).WithMessage("La nueva contraseña debe tener al menos 6 caracteres")
                .MaximumLength(50).WithMessage("La nueva contraseña no puede exceder 50 caracteres")
                .NotEqual(x => x.CurrentPassword).WithMessage("La nueva contraseña debe ser diferente a la actual");
        }
    }
}

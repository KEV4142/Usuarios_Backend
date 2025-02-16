using FluentValidation;

namespace Aplicacion.Tablas.Accounts.UsuarioRestablecerPassword;
public class UsuarioRestablecerPasswordValidator : AbstractValidator<UsuarioRestablecerPasswordRequest>
{
    public UsuarioRestablecerPasswordValidator()
    {
        RuleFor(x => x.Password)
        .Cascade(CascadeMode.Stop)
        .NotEmpty().WithMessage("El campo password esta en blanco.")
        .MinimumLength(8).WithMessage("El campo password debe tener al menos 8 caracteres.")
        .Matches(@"[A-Z]").WithMessage("El campo password debe contener al menos una letra mayúscula.")
        .Matches(@"[\W_]").WithMessage("El campo password debe contener al menos un carácter especial.")
        .Matches(@"\d").WithMessage("El campo password debe contener al menos un dígito.");
    }
}

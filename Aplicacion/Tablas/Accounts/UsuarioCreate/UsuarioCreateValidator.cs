using FluentValidation;

namespace Aplicacion.Tablas.Accounts.UsuarioCreate;
public class UsuarioCreateValidator : AbstractValidator<UsuarioCreateRequest>
{
    public UsuarioCreateValidator()
    {
        RuleFor(x => x.Email)
        .Cascade(CascadeMode.Stop)
        .NotEmpty().WithMessage("El Campo Email esta vacio.")
        .EmailAddress().WithMessage("El Email ingresado no es formato correcto.");

        RuleFor(x => x.Password)
        .Cascade(CascadeMode.Stop)
        .MinimumLength(8).WithMessage("El campo password debe tener al menos 8 caracteres.")
        .Matches(@"[A-Z]").WithMessage("El campo password debe contener al menos una letra mayúscula.")
        .Matches(@"[\W_]").WithMessage("El campo password debe contener al menos un carácter especial.")
        .Matches(@"\d").WithMessage("El campo password debe contener al menos un dígito.");

        RuleFor(x => x.NombreCompleto).NotEmpty().WithMessage("El campo nombre se encuentra vacio.");

        RuleFor(x => x.Username).NotEmpty().WithMessage("Ingrese un username.");
    }
}

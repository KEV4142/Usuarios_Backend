using FluentValidation;

namespace Aplicacion.Tablas.Accounts.UsuarioForgotPassword;
public class UsuarioForgotPasswordValidator : AbstractValidator<UsuarioForgotPasswordRequest>
{
    public UsuarioForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
        .Cascade(CascadeMode.Stop)
        .NotNull().WithMessage("El campo Email es requerido.")
        .NotEmpty().WithMessage("El Campo Email esta vacio.")
        .EmailAddress().WithMessage("El Email ingresado no es formato correcto.");
    }
}

using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Seguridad.Interfaces;

namespace Aplicacion.Tablas.Accounts.UsuarioForgotPassword;
public class UsuarioForgotPasswordCommand
{
    public record UsuarioForgotPasswordCommandRequest(UsuarioForgotPasswordRequest usuarioForgotPasswordRequest) : IRequest<Result<Boolean>>;

    internal class UsuarioForgotPasswordCommandHandler : IRequestHandler<UsuarioForgotPasswordCommandRequest, Result<Boolean>>
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly UrlHelperService _urlHelperService;

        public UsuarioForgotPasswordCommandHandler(UserManager<Usuario> userManager, ITokenService tokenService, IEmailSender emailSender, UrlHelperService urlHelperService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _urlHelperService = urlHelperService;
        }

        public async Task<Result<Boolean>> Handle(UsuarioForgotPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == request.usuarioForgotPasswordRequest.Email!.ToUpper());

            if (usuario is null)
            {
                return Result<Boolean>.Failure("No se tiene ningún Usuario con ese email.", HttpStatusCode.NotFound);
            }
            if (!usuario.EmailConfirmed)
            {
                return Result<Boolean>.Failure("Debes confirmar tu email antes de cualquier proceso.", HttpStatusCode.Forbidden);
            }
            var token = _tokenService.CreateEmailConfirmationToken(usuario);
            var resetPasswordUrl = _urlHelperService.GenerateChangePasswordEmailUrl(usuario.Id, token);

            var correo = $@"
            <p>Hola {Funciones.ToProperCase(usuario.NombreCompleto)},</p>
            <p>Recibimos una solicitud para restablecer tu contraseña.</p>
            <p>Haz clic en el siguiente enlace para cambiar tu contraseña:</p>
            <p><a href='{resetPasswordUrl}'>Restablecer contraseña</a></p>
            <p>Si no solicitaste este cambio, ignora este mensaje.</p>
            ";

            var envioEmail = await _emailSender.SendEmailAsync(usuario.Email!, "Restablecimiento de contraseña", correo);

            if (!envioEmail)
            {
                return Result<Boolean>.Failure("Hubo un error al enviar el email de restablecimiento de contraseña.", HttpStatusCode.InternalServerError);
            }
            return Result<Boolean>.Success(true);
        }
    }
    public class UsuarioForgotPasswordCommandRequestValidator : AbstractValidator<UsuarioForgotPasswordCommandRequest>
    {
        public UsuarioForgotPasswordCommandRequestValidator()
        {
            RuleFor(x => x.usuarioForgotPasswordRequest).SetValidator(new UsuarioForgotPasswordValidator());
        }
    }
}

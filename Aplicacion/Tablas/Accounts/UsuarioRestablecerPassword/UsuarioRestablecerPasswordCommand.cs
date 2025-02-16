using System.Net;
using System.Security.Claims;
using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Accounts.UsuarioConfirmacion;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Seguridad.Interfaces;

namespace Aplicacion.Tablas.Accounts.UsuarioRestablecerPassword;
public class UsuarioRestablecerPasswordCommand
{
    public record UsuarioRestablecerPasswordCommandRequest(UsuarioRestablecerPasswordRequest usuarioRestablecerPasswordRequest) : IRequest<Result<UsuarioResponse>>;

    internal class UsuarioRestablecerPasswordCommandHandler : IRequestHandler<UsuarioRestablecerPasswordCommandRequest, Result<UsuarioResponse>>
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuarioRestablecerPasswordCommandHandler(UserManager<Usuario> userManager, ITokenService tokenService, IEmailSender emailSender, IPasswordHasher<Usuario> passwordHasher)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<UsuarioResponse>> Handle(UsuarioRestablecerPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            if (!_tokenService.ValidateEmailConfirmationToken(request.usuarioRestablecerPasswordRequest.token!, out var principal))
            {
                return Result<UsuarioResponse>.Failure("Token inválido o expirado.", HttpStatusCode.Unauthorized);
            }

            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim != request.usuarioRestablecerPasswordRequest.userId!)
            {
                return Result<UsuarioResponse>.Failure("El token no pertenece al usuario.", HttpStatusCode.Unauthorized);
            }
            var usuario = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.usuarioRestablecerPasswordRequest.userId!);
            if (usuario is null)
            {
                return Result<UsuarioResponse>.Failure("El Usuario no existe", HttpStatusCode.NotFound);
            }

            if (request.usuarioRestablecerPasswordRequest.Password is not null)
            {
                usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.usuarioRestablecerPasswordRequest.Password!);
            }

            var resultado = await _userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
            {
                return Result<UsuarioResponse>.Failure("Errores en la Actualización de la Contraseña.", HttpStatusCode.BadRequest);
            }

            var correo = $@"
                <p>Buen día {Funciones.ToProperCase(usuario.NombreCompleto)},</p>
                <p>Se realizó el restablecimiento de la contraseña con éxito.</p>
                <p>Gracias por su tiempo.</p>";

            var envioEmail = await _emailSender.SendEmailAsync(usuario.Email!, "Restablecimiento de contraseña exitoso", correo);

            if (!envioEmail)
            {
                return Result<UsuarioResponse>.Failure("Hubo un error al enviar el email pero de igual forma se restableció la contraseña recibida.", HttpStatusCode.InternalServerError);
            }

            var profile = new UsuarioResponse
            (
                usuario.Id,
                usuario.NombreCompleto,
                usuario.Email!,
                usuario.UserName!
            );

            return Result<UsuarioResponse>.Success(profile);
        }
    }
    public class UsuarioRestablecerPasswordCommandRequestValidator : AbstractValidator<UsuarioRestablecerPasswordCommandRequest>
    {
        public UsuarioRestablecerPasswordCommandRequestValidator()
        {
            RuleFor(x => x.usuarioRestablecerPasswordRequest).SetValidator(new UsuarioRestablecerPasswordValidator());
        }
    }
}

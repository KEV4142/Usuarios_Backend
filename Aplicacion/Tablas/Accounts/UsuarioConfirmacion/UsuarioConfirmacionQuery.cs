using System.Net;
using System.Security.Claims;
using Aplicacion.Core;
using Aplicacion.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Modelo.Entidades;
using Seguridad.Interfaces;

namespace Aplicacion.Tablas.Accounts.UsuarioConfirmacion;
public class UsuarioConfirmacionQuery
{
    public record UsuarioConfirmacionQueryRequest : IRequest<Result<UsuarioResponse>>
    {
        public UsuarioConfirmacionRequest? usuarioConfirmacionQueryRequest { get; set; }
    }
    internal class UsuarioConfirmacionQueryHandler
    : IRequestHandler<UsuarioConfirmacionQueryRequest, Result<UsuarioResponse>>
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;

        public UsuarioConfirmacionQueryHandler(UserManager<Usuario> userManager, ITokenService tokenService, IEmailSender emailSender)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailSender = emailSender;
        }

        public async Task<Result<UsuarioResponse>> Handle(
            UsuarioConfirmacionQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            var id = request.usuarioConfirmacionQueryRequest!.userId;
            var token = request.usuarioConfirmacionQueryRequest!.token;

            if (string.IsNullOrEmpty(token))
            {
                return Result<UsuarioResponse>.Failure("ID de usuario o token no proporcionados.", HttpStatusCode.BadRequest);
            }
            if (id is not null)
            {
                if (!_tokenService.ValidateEmailConfirmationToken(token, out var principal))
                {
                    return Result<UsuarioResponse>.Failure("Token inválido o expirado.", HttpStatusCode.Unauthorized);
                }

                var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdClaim != id)
                {
                    return Result<UsuarioResponse>.Failure("El token no pertenece al usuario.", HttpStatusCode.Unauthorized);
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user is null)
                {
                    return Result<UsuarioResponse>.Failure("No se encontro el usuario.", HttpStatusCode.NotFound);
                }
                if (user.EmailConfirmed)
                {
                    return Result<UsuarioResponse>.Failure("El email ya ha sido confirmado previamente.", HttpStatusCode.Conflict);
                }
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                var profile = new UsuarioResponse
                (
                    user.Id,
                    user.NombreCompleto,
                    user.Email!,
                    user.UserName!
                );
                bool emailEnviado = await _emailSender.SendEmailAsync(user.Email!, "Su cuenta ha sido validada",
                            $"Hola {Funciones.ToProperCase(user.NombreCompleto)},<br>Felicitaciones, la cuenta ha sido validada con exito y esta lista para su uso.<br>Solamente y gracias por su tiempo.");

                if (!emailEnviado)
                {
                    return Result<UsuarioResponse>.Failure("Usuario dado de alta, pero hubo un error al enviar el email de bienvenida.", HttpStatusCode.InternalServerError);
                }
                return Result<UsuarioResponse>.Success(profile);
            }
            return Result<UsuarioResponse>.Failure("No se tiene ID de Usuario.", HttpStatusCode.BadRequest);
        }
    }
}
public record UsuarioResponse(
    string UsuarioID,
    string NombreCompleto,
    string Email,
    string Username
);
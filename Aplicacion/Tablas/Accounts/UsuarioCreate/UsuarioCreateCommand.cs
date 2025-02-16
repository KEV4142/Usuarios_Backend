using System;
using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelo.Custom;
using Modelo.Entidades;
using Seguridad.Interfaces;

namespace Aplicacion.Tablas.Accounts.UsuarioCreate;
public class UsuarioCreateCommand
{
    public record UsuarioCreateCommandRequest(UsuarioCreateRequest usuarioCreateRequest) : IRequest<Result<Profile>>;

    internal class UsuarioCreateCommandHandler : IRequestHandler<UsuarioCreateCommandRequest, Result<Profile>>
    {

        private readonly UserManager<Usuario> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly UrlHelperService _urlHelperService;

        public UsuarioCreateCommandHandler(
            UserManager<Usuario> userManager,
            ITokenService tokenService,
            IEmailSender emailSender,
            UrlHelperService urlHelperService
        )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _urlHelperService = urlHelperService;
        }

        public async Task<Result<Profile>> Handle(UsuarioCreateCommandRequest request, CancellationToken cancellationToken)
        {
            var tipo = CustomRoles.CLIENT;

            if (await _userManager.Users
            .AnyAsync(x => x.Email == request.usuarioCreateRequest.Email!.ToUpper()))
            {
                return Result<Profile>.Failure("El email ya fue registrado por otro usuario", HttpStatusCode.Conflict);
            }

            if (await _userManager.Users
            .AnyAsync(x => x.UserName == request.usuarioCreateRequest.Username!.ToUpper()))
            {
                return Result<Profile>.Failure("El username ya fue registrado", HttpStatusCode.Conflict);
            }

            var user = new Usuario
            {
                NombreCompleto = request.usuarioCreateRequest.NombreCompleto!.ToUpper(),
                Id = Guid.NewGuid().ToString(),
                Email = request.usuarioCreateRequest.Email!.ToUpper(),
                UserName = request.usuarioCreateRequest.Username!.ToUpper()
            };

            var resultado = await _userManager
            .CreateAsync(user, request.usuarioCreateRequest.Password!);

            if (!resultado.Succeeded)
            {
                return Result<Profile>.Failure("Errores en el registro del nuevo usuario.", HttpStatusCode.BadRequest);
            }

            await _userManager.AddToRoleAsync(user, tipo);
            var token = _tokenService.CreateEmailConfirmationToken(user);
            var confirmationLink = _urlHelperService.GenerateConfirmEmailUrl(user.Id, token.ToString()!);

            bool emailEnviado = await _emailSender.SendEmailAsync(user.Email!, "Confirma tu cuenta",
                            $"Buen día,<br>Haz click en este enlace para confirmar tu cuenta: <a href='{confirmationLink}'>Confirmar Email</a>");

            if (!emailEnviado)
            {
                return Result<Profile>.Failure("Usuario registrado, pero hubo un error al enviar el email de confirmación.", HttpStatusCode.InternalServerError);
            }
            var profile = new Profile
            {
                Email = user.Email,
                NombreCompleto = user.NombreCompleto,
                Token = await _tokenService.CreateToken(user),
                Username = user.UserName,
                Tipo = "Operador"
            };

            return Result<Profile>.Success(profile);
        }
    }
    public class RegiterCommandRequestValidator : AbstractValidator<UsuarioCreateCommandRequest>
    {
        public RegiterCommandRequestValidator()
        {
            RuleFor(x => x.usuarioCreateRequest).SetValidator(new UsuarioCreateValidator());
        }
    }
}

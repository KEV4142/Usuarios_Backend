using System.Net;
using Aplicacion.Core;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelo.Custom;
using Modelo.Entidades;
using Seguridad.Interfaces;

namespace Aplicacion.Tablas.Accounts.Login;
public class LoginCommand
{
    public record LoginCommandRequest(LoginRequest loginRequest) : IRequest<Result<Profile>>;

    internal class LoginCommandHandler
        : IRequestHandler<LoginCommandRequest, Result<Profile>>
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(
            UserManager<Usuario> userManager,
            ITokenService tokenService
        )
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<Profile>> Handle(
            LoginCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Email == request.loginRequest.Email!.ToUpper());

            if (user is null)
            {
                return Result<Profile>.Failure("No se encontro el usuario", HttpStatusCode.NotFound);
            }
            if (!user.EmailConfirmed)
            {
                return Result<Profile>.Failure("Debes confirmar tu email antes de iniciar sesión.", HttpStatusCode.Forbidden);
            }

            var resultado = await _userManager
            .CheckPasswordAsync(user, request.loginRequest.Password!);

            if (!resultado)
            {
                return Result<Profile>.Failure("Las credenciales son incorrectas", HttpStatusCode.Unauthorized);
            }
            var roleNames = await _userManager.GetRolesAsync(user);
            var tipo = roleNames.Contains(CustomRoles.ADMIN) ? "Administrador" :
                       roleNames.Contains(CustomRoles.CLIENT) ? "Operador" :
                       "Sin Asignacion de Rol.";

            var profile = new Profile
            {
                Email = user.Email,
                NombreCompleto = user.NombreCompleto,
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                Tipo = tipo
            };

            return Result<Profile>.Success(profile);
        }
    }
    public class LoginCommandRequestValidator : AbstractValidator<LoginCommandRequest>
    {
        public LoginCommandRequestValidator()
        {
            RuleFor(x => x.loginRequest).SetValidator(new LoginValidator());
        }
    }
}

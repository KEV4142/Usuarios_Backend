using System.Security.Claims;
using Modelo.Entidades;

namespace Seguridad.Interfaces;
public interface ITokenService
{
    Task<string> CreateToken(Usuario user);
    public string CreateEmailConfirmationToken(Usuario user);
    public bool ValidateEmailConfirmationToken(string token, out ClaimsPrincipal? principal);
}

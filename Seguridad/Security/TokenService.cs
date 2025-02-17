using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Modelo.Entidades;
using Seguridad.Interfaces;

namespace Seguridad.Security;
public class TokenService : ITokenService
{
    private readonly IAESCrypto _aesCrypto;
    private readonly IConfiguration _configuration;
    private readonly UserManager<Usuario> _userManager;

    public TokenService(IConfiguration configuration, UserManager<Usuario> userManager, IAESCrypto aesCrypto)
    {
        _configuration = configuration;
        _userManager = userManager;
        _aesCrypto = aesCrypto;
    }

    public async Task<string> CreateToken(Usuario usuario)
    {
        var roles = await _userManager.GetRolesAsync(usuario);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, _aesCrypto.Encrypt(usuario.UserName!)),
            new Claim(ClaimTypes.NameIdentifier, _aesCrypto.Encrypt(usuario.Id)),
            new Claim(ClaimTypes.Email, _aesCrypto.Encrypt(usuario.Email!))
        };

        foreach (var role in roles)
        {
            if (role is not null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenKey"]!)),
            SecurityAlgorithms.HmacSha256
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
    public string CreateEmailConfirmationToken(Usuario usuario)
    {
        var claims = new List<Claim>{
            new Claim(ClaimTypes.NameIdentifier, _aesCrypto.Encrypt(usuario.Id)),
            new Claim(ClaimTypes.Email, _aesCrypto.Encrypt(usuario.Email!))
        };
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenKey"]!)),
            SecurityAlgorithms.HmacSha256
        );
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = creds
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public bool ValidateEmailConfirmationToken(string token, out ClaimsPrincipal? principal)
    {
        principal = null;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["TokenKey"]!);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            var validatedPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

            var claimsIdentity = validatedPrincipal.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var encryptedId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var encryptedEmail = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;

                var decryptedClaims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier, _aesCrypto.Decrypt(encryptedId!)),
                new Claim(ClaimTypes.Email, _aesCrypto.Decrypt(encryptedEmail!)),
                };
                var newClaimsIdentity = new ClaimsIdentity(decryptedClaims, claimsIdentity.AuthenticationType);
                principal = new ClaimsPrincipal(newClaimsIdentity);
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al validar el token: {ex.Message}");
            return false;
        }
    }

}
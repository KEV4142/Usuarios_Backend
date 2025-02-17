using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Modelo.Entidades;
using Repositorio;
using Seguridad.Core;
using Seguridad.Interfaces;
using Seguridad.Security;

namespace WebApi.Extensions;
public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddIdentityCore<Usuario>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.User.RequireUniqueEmail = true;
        }).AddRoles<IdentityRole>().AddEntityFrameworkStores<BackendContext>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserAccessor, UserAccessor>();
        services.AddScoped<IAESCrypto, AESCrypto>();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]!));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            opt.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var aesCrypto = context.HttpContext.RequestServices.GetRequiredService<IAESCrypto>();
                    var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;

                    if (claimsIdentity != null)
                    {
                        var encryptedName = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                        var encryptedEmail = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
                        var encryptedId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        if (!string.IsNullOrEmpty(encryptedName))
                        {
                            claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(ClaimTypes.Name));
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, aesCrypto.Decrypt(encryptedName)));
                        }

                        if (!string.IsNullOrEmpty(encryptedEmail))
                        {
                            claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(ClaimTypes.Email));
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, aesCrypto.Decrypt(encryptedEmail)));
                        }

                        if (!string.IsNullOrEmpty(encryptedId))
                        {
                            claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier));
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, aesCrypto.Decrypt(encryptedId)));
                        }
                    }

                    await Task.CompletedTask;
                }
            };
        });
        return services;
    }
}

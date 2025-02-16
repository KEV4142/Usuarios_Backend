using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Aplicacion.Core;

public class UrlHelperService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _frontendBaseUrl;

    public UrlHelperService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _frontendBaseUrl = configuration["AllowedFrontendHosts"]  ?? "http://localhost";
    }

    public string GenerateConfirmEmailUrl(string userId, string token)
    {
        var confirmationUrl = $"{_frontendBaseUrl}/confirmarCuenta?userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}";

        return confirmationUrl;
    }
    public string GenerateChangePasswordEmailUrl(string userId, string token)
    {
        var confirmationUrl = $"{_frontendBaseUrl}/reinicioPassword?userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}";

        return confirmationUrl;
    }
    public string GenerateConfirmEmailUrlBackend(string userId, string token)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var baseUrl = $"{httpContext!.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}";
        var confirmationUrl = $"{baseUrl}/api/account/confirmemail?userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}";

        return confirmationUrl;
    }
}

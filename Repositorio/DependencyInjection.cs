using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Repositorio;
public static class DependencyInjection
{
    public static IServiceCollection AddRepositorio(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<BackendContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        return services;
    }
}
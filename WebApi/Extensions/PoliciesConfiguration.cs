using Modelo.Custom;

namespace WebApi.Extensions;
public static class PoliciesConfiguration
{
    public static IServiceCollection AddPoliciesServices(
        this IServiceCollection services
    )
    {
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(
                CustomRoles.ADMIN, policy =>
                   policy.RequireRole(CustomRoles.ADMIN)
            );
        });
        return services;
    }
}

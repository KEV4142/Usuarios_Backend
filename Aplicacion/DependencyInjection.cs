
using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Accounts.Login;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Aplicacion;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicacion(
        this IServiceCollection services
    )
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            //configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<LoginCommand>();
        // services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddSingleton<UrlHelperService>();
        return services;
    }
}

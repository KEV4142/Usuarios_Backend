using System.Net;
using Aplicacion.Tablas.Accounts;
using Aplicacion.Tablas.Accounts.GetCurrentUser;
using Aplicacion.Tablas.Accounts.Login;
using Aplicacion.Tablas.Accounts.UsuarioConfirmacion;
using Aplicacion.Tablas.Accounts.UsuarioCreate;
using Aplicacion.Tablas.Accounts.UsuarioForgotPassword;
using Aplicacion.Tablas.Accounts.UsuarioRestablecerPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seguridad.Interfaces;
using static Aplicacion.Tablas.Accounts.GetCurrentUser.GetCurrentUserQuery;
using static Aplicacion.Tablas.Accounts.Login.LoginCommand;
using static Aplicacion.Tablas.Accounts.UsuarioConfirmacion.UsuarioConfirmacionQuery;
using static Aplicacion.Tablas.Accounts.UsuarioCreate.UsuarioCreateCommand;
using static Aplicacion.Tablas.Accounts.UsuarioForgotPassword.UsuarioForgotPasswordCommand;
using static Aplicacion.Tablas.Accounts.UsuarioRestablecerPassword.UsuarioRestablecerPasswordCommand;

namespace WebApi.Controller;
[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserAccessor _user;

    public AccountController(ISender sender, IUserAccessor user)
    {
        _sender = sender;
        _user = user;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<Profile>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new LoginCommandRequest(request);
        var resultado = await _sender.Send(command, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<Profile>> Me(CancellationToken cancellationToken)
    {
        var email = _user.GetEmail();
        var request = new GetCurrentUserRequest { Email = email };
        var query = new GetCurrentUserQueryRequest(request);
        var resultado = await _sender.Send(query, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);
    }
    [AllowAnonymous]
    [HttpPost("agregar")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Profile>> AgregarUsuario(
        [FromBody] UsuarioCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UsuarioCreateCommandRequest(request);
        var resultado = await _sender.Send(command, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);
    }

    [AllowAnonymous]
    [HttpGet("confirmemail")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<UsuarioResponse>> ConfirmEmail(
        [FromQuery] UsuarioConfirmacionRequest request,
        CancellationToken cancellationToken
        )
    {
        var query = new UsuarioConfirmacionQueryRequest { usuarioConfirmacionQueryRequest = request };
        var resultado = await _sender.Send(query, cancellationToken);

        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);
    }

    [AllowAnonymous]
    [HttpPost("restablecer")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Boolean>> RestablecerPassword(
        [FromBody] UsuarioForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UsuarioForgotPasswordCommandRequest(request);
        var resultado = await _sender.Send(command, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);
    }

    [AllowAnonymous]
    [HttpPut("restablecerpassword")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<UsuarioResponse>> ActualizarPasswordTokenUsuario(
    [FromBody] UsuarioRestablecerPasswordRequest request,
    [FromQuery] string id,
    [FromQuery] string token,
    CancellationToken cancellationToken
)
    {
        request.userId = id;
        request.token = token;
        var command = new UsuarioRestablecerPasswordCommandRequest(request);
        var resultado = await _sender.Send(command, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);
    }
}

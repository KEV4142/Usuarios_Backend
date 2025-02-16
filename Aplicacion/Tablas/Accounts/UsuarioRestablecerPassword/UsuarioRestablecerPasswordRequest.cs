namespace Aplicacion.Tablas.Accounts.UsuarioRestablecerPassword;
public class UsuarioRestablecerPasswordRequest
{
    public string? userId { get; set; }
    public string? token { get; set; }
    public string? Password { get; set; }
}

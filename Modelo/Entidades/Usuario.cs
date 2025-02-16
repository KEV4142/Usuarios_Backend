using Microsoft.AspNetCore.Identity;

namespace Modelo.Entidades;
public class Usuario : IdentityUser
{
    public string NombreCompleto {get;set;} = null!;
}

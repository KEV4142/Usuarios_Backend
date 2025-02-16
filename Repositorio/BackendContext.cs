using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modelo.Custom;
using Modelo.Entidades;

namespace Repositorio;

public partial class BackendContext : IdentityDbContext<Usuario>
{
    public BackendContext()
    {
    }

    public BackendContext(DbContextOptions<BackendContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var adminRoleId = "51df7aae-a506-46ff-8e34-9f2f0c661885";
        var clientRoleId = "368cb24e-03d3-4a01-b558-dbde9b33272c";


        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = CustomRoles.ADMIN,
                    NormalizedName = CustomRoles.ADMIN
                },
                new IdentityRole
                {
                    Id = clientRoleId,
                    Name = CustomRoles.CLIENT,
                    NormalizedName = CustomRoles.CLIENT
                }
            );

    }

}

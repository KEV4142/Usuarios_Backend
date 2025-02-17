using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modelo.Custom;
using Modelo.Entidades;
using Seguridad.Interfaces;

namespace Repositorio;

public partial class BackendContext : IdentityDbContext<Usuario>
{
    private readonly IAESCrypto _AESCrypto;
    public BackendContext(IAESCrypto AESCrypto)
    {
        _AESCrypto = AESCrypto;
    }

    public BackendContext(DbContextOptions<BackendContext> options,IAESCrypto AESCrypto)
        : base(options)
    {
        _AESCrypto = AESCrypto;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var adminRoleId = "51df7aae-a506-46ff-8e34-9f2f0c661885";
        var clientRoleId = "368cb24e-03d3-4a01-b558-dbde9b33272c";


        base.OnModelCreating(modelBuilder);
        
        var encryptionConverter = new ValueConverter<string?, string?>(
            v => v == null ? null : _AESCrypto.Encrypt(v),
            v => v == null ? null : _AESCrypto.Decrypt(v)
        );
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Email)
            .HasConversion(encryptionConverter);

        modelBuilder.Entity<Usuario>()
            .Property(u => u.UserName)
            .HasConversion(encryptionConverter);


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

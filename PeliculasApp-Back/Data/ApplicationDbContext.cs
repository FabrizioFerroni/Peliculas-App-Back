using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeliculasApp_Back.Entidades;

namespace PeliculasApp_Back.Data;

public class ApplicationDbContext: IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions options): base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<IdentityUser<Guid>>(entity => entity.ToTable(name: "Usuarios"));
        modelBuilder.Entity<IdentityRole<Guid>>(entity => entity.ToTable(name: "Roles"));
        modelBuilder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("UsuarioRoles"));
        modelBuilder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable("UsuarioClaims"));
        modelBuilder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("UsuarioLogins"));
        modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("RolClaims"));
        modelBuilder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("UsuarioTokens"));
        
        modelBuilder.Entity<PeliculaGenero>().HasKey(e => new { e.GeneroId, e.PeliculaId});
        modelBuilder.Entity<PeliculaCine>().HasKey(e => new { e.CineId, e.PeliculaId});
        modelBuilder.Entity<PeliculaActor>().HasKey(e => new { e.ActorId, e.PeliculaId});
        
    }

    public DbSet<Genero> Generos { get; set; }
    public DbSet<Actor> Actores { get; set; }
    public DbSet<Cine> Cines { get; set; }
    public DbSet<Pelicula> Peliculas { get; set; }
    public DbSet<PeliculaGenero> PeliculaGeneros { get; set; }
    public DbSet<PeliculaCine> PeliculaCines { get; set; }
    public DbSet<PeliculaActor> PeliculaActores { get; set; }
    public DbSet<Rating> RatingsPeliculas { get; set; }
}
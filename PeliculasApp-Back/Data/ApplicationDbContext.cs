using Microsoft.EntityFrameworkCore;
using PeliculasApp_Back.Entidades;

namespace PeliculasApp_Back.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions options): base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
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
}
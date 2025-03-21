using System.ComponentModel.DataAnnotations;

namespace PeliculasApp_Back.Entidades;

public class PeliculaActor
{
    public Guid ActorId {get; set;}
    public Guid PeliculaId {get; set;}
    [StringLength(300)]
    public required string Personaje {get; set;}
    public int Orden {get; set;}
    public Actor Actor { get; set; } = null!;
    public Pelicula Pelicula { get; set; } = null!;
}
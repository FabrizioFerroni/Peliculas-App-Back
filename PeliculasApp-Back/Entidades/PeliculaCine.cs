namespace PeliculasApp_Back.Entidades;

public class PeliculaCine
{
    public Guid CineId {get; set;}
    public Guid PeliculaId {get; set;}
    public Cine Cine { get; set; } = null!;
    public Pelicula Pelicula { get; set; } = null!;
}
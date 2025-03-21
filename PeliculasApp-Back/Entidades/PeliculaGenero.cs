namespace PeliculasApp_Back.Entidades;

public class PeliculaGenero
{
    public Guid GeneroId {get; set;}
    public Guid PeliculaId {get; set;}
    public Genero Genero { get; set; } = null!;
    public Pelicula Pelicula { get; set; } = null!;
}
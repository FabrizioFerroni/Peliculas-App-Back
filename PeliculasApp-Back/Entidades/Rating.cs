using Microsoft.AspNetCore.Identity;

namespace PeliculasApp_Back.Entidades;

public class Rating
{
    public Guid Id { get; set; }
    public int Puntuacion { get; set; }
    public Guid PeliculaId { get; set; }
    public required Guid UsuarioId { get; set; }
    public Pelicula Pelicula { get; set; } = null!;
    public IdentityUser<Guid> Usuario { get; set; } = null!;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaModificacion { get; set; }
}
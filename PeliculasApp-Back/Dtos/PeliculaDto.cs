using PeliculasApp_Back.Entidades;

namespace PeliculasApp_Back.Dtos;

public class PeliculaDto : IId
{
    public Guid Id { get; set; }
    public required string Titulo { get; set; } = string.Empty;
    public string? Slug { get; set; } = string.Empty;
    public string? Descripcion { get; set; } = string.Empty;
    public string? Trailer { get; set; } = string.Empty;
    public DateTime FechaLanzamiento { get; set; }
    public string? Poster { get; set; } = string.Empty; 
    public int Duracion { get; set; }

    public string AnioLanzamiento { get; set; } = string.Empty;
    public bool EnCines { get; set; }
    public bool ProximoEstreno { get; set; }
    public string Director { get; set; } = string.Empty;
    public double PromedioVoto  { get; set; }
    public int VotoUsuario { get; set; }
}
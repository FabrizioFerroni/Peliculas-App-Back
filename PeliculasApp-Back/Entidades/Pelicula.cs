using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PeliculasApp_Back.Validaciones;

namespace PeliculasApp_Back.Entidades;

public class Pelicula : IId
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
    [PrimeraLetraMayuscula]
    public required string Titulo { get; set; } = string.Empty;

    public string? Slug { get; set; } = string.Empty;
    public string? Descripcion { get; set; } = string.Empty;
    public string? Trailer { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es requerido")]
    public DateTime FechaLanzamiento { get; set; }

    [Unicode(false)] public string? Poster { get; set; } = string.Empty;
    public List<PeliculaGenero> PeliculasGeneros { get; set; } = new List<PeliculaGenero>();
    public List<PeliculaCine> PeliculasCines { get; set; } = new List<PeliculaCine>();
    public List<PeliculaActor> PeliculasActores { get; set; } = new List<PeliculaActor>();
    public int Duracion { get; set; }

    public string AnioLanzamiento { get; set; } = string.Empty;
    public bool EnCines { get; set; }
    public bool ProximoEstreno { get; set; }
    public string Director { get; set; } = string.Empty;

public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaModificacion { get; set; } 

}
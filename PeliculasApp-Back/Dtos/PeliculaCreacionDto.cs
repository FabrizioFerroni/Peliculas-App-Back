using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using PeliculasApp_Back.Utilidades;
using PeliculasApp_Back.Validaciones;

namespace PeliculasApp_Back.Dtos;

public class PeliculaCreacionDto
{
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
    [PrimeraLetraMayuscula]
    public required string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; } = string.Empty;
    public string? Trailer { get; set; } = string.Empty;
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public DateTime FechaLanzamiento { get; set; }
    public IFormFile? Poster { get; set; } = null;
    [ModelBinder(BinderType = typeof(TypeBinder))]
    public List<Guid>? GenerosIds { get; set; }
    [ModelBinder(BinderType = typeof(TypeBinder))]
    public List<Guid>? CinesIds { get; set; }
    [ModelBinder(BinderType = typeof(TypeBinder))]
    public List<ActorPeliculaCreacionDto>? Actores { get; set; }
    
    public int Duracion { get; set; }

    public string AnioLanzamiento { get; set; } = string.Empty;
    public bool EnCines { get; set; }
    public bool ProximoEstreno { get; set; }
    public string Director { get; set; } = string.Empty;
}
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PeliculasApp_Back.Validaciones;

namespace PeliculasApp_Back.Entidades;


[Index(nameof(Nombre), IsUnique = true)]
public class Cine: IId
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
    [PrimeraLetraMayuscula]
    public required string Nombre { get; set; } = string.Empty;
    
    public string? Slug { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public required Point Ubicacion { get; set; }
    
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaModificacion { get; set; } 

}
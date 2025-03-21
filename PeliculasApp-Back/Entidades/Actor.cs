using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PeliculasApp_Back.Validaciones;

namespace PeliculasApp_Back.Entidades;

[Index(nameof(Nombre), IsUnique = true)]
public class Actor: IId
{
    
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
    [PrimeraLetraMayuscula]
    public required string Nombre { get; set; } = string.Empty;
    public string? Slug { get; set; } = string.Empty;
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public DateTime FechaNacimiento { get; set; }
    [Unicode(false)]
    public string? Foto { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaModificacion { get; set; }
}
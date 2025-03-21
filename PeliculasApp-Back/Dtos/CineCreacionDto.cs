using System.ComponentModel.DataAnnotations;
using PeliculasApp_Back.Validaciones;

namespace PeliculasApp_Back.Dtos;

public class CineCreacionDto
{
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
    [PrimeraLetraMayuscula]
    public required string Nombre { get; set; }
    public string? Slug { get; set; }
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [Range(-90, 90)]
    public double Latitud { get; set; }
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [Range(-180, 180)]
    public double Longitud { get; set; }
}
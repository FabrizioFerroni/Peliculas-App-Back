using System.ComponentModel.DataAnnotations;
using PeliculasApp_Back.Validaciones;

namespace PeliculasApp_Back.Dtos;

public class GeneroCreacionDto
{
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
    [PrimeraLetraMayuscula]
    public required string Nombre { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace PeliculasApp_Back.Dtos;

public class RatingCreacionDto
{
    public Guid PeliculaId { get; set; }
    [Required]
    [Range(1, 5)]
    public int Puntuacion { get; set; }
}
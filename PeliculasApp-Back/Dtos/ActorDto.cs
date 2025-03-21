using PeliculasApp_Back.Entidades;

namespace PeliculasApp_Back.Dtos;

public class ActorDto: IId
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }
    public string? Slug { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string? Foto { get; set; }
}
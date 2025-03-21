namespace PeliculasApp_Back.Dtos;

public class ActorPeliculaCreacionDto
{
    public Guid Id { get; set; }
    public required string Personaje { get; set; }
}
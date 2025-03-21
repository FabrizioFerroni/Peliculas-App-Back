namespace PeliculasApp_Back.Dtos;

public class PeliculaActorDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string? Foto { get; set; }
    public string Personaje { get; set; }
}
namespace PeliculasApp_Back.Entidades;

public class Genero
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Nombre { get; set; } = string.Empty;
}
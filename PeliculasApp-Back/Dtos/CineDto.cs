using PeliculasApp_Back.Entidades;

namespace PeliculasApp_Back.Dtos;

public class CineDto : IId
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }
    public string? Slug { get; set; }
    public double Latitud { get; set; }
    public double Longitud { get; set; }
}
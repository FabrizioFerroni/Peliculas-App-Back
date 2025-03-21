namespace PeliculasApp_Back.Entidades;

public interface IId
{
    public Guid Id { get; set; }
    public string? Slug { get; set; }
}
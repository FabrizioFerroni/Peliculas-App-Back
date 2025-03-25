namespace PeliculasApp_Back.Dtos;

public class UsuarioDTO
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public required string Email { get; set; }
}
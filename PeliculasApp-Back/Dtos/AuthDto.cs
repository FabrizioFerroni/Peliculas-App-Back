namespace PeliculasApp_Back.Dtos;

public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class RegisterDto
{
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
}

public class RespuestaAuthDto
{
    public required string Token { get; set; }
    public DateTime TokenExpiry { get; set; }
}
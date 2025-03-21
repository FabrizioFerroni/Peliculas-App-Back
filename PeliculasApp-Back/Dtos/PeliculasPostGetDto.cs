namespace PeliculasApp_Back.Dtos;

public class PeliculasPostGetDto
{
    public List<GeneroDto> Generos { get; set; } = new List<GeneroDto>();
    public List<CineDto> Cines { get; set; } = new List<CineDto>();
}
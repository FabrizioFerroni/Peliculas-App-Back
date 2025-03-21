namespace PeliculasApp_Back.Dtos;

public class PeliculaDetallesDto: PeliculaDto
{
    public List<GeneroDto> Generos { get; set; } = new List<GeneroDto>();
    public List<CineDto> Cines { get; set; } = new List<CineDto>();
    public List<PeliculaActorDto> Actores { get; set; } = new List<PeliculaActorDto>();
}
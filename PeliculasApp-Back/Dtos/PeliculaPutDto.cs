namespace PeliculasApp_Back.Dtos;

public class PeliculaPutDto
{
    public PeliculaDto Pelicula { get; set; } = null!;
    public List<GeneroDto> GenerosSeleccionados { get; set; } = new List<GeneroDto>();
    public List<GeneroDto> GenerosNoSeleccionados { get; set; } = new List<GeneroDto>();
    public List<CineDto> CineSeleccionados { get; set; } = new List<CineDto>();
    public List<CineDto> CineNoSeleccionados { get; set; } = new List<CineDto>();
    public List<PeliculaActorDto> Actores  { get; set; } = new List<PeliculaActorDto>();
}
namespace PeliculasApp_Back.Dtos;

public class LandingPageDto
{
    public List<PeliculaDto> EnCines { get; set; } = new List<PeliculaDto>();
    public List<PeliculaDto> ProximosEstrenos { get; set; } = new List<PeliculaDto>();
}